using AutoMapper;
using OneOf;
using Microsoft.Extensions.Options;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Data.Repositories;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Infrastructure.OneOfResults;
using RecipeRepository.Logic.Infrastructure.Settings;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Logic.Services;

public class UserService(RecipeRepoContext context, IMapper mapper, IOptions<AppSettings> appSettings, IFileService fileService) : IUserService
{
    private readonly UserRepository _userRepository = new(context);
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<IEnumerable<AppUser>> GetUsers()
        => mapper.Map<IEnumerable<AppUser>>(await _userRepository.GetUsers());

    public async Task<AppUser?> GetUser(string id)
    {
        return mapper.Map<AppUser?>(await _userRepository.GetUser(id));
    }

    public async Task<OneOf<AppUser, BadRequest, NotFound, Error>> UpdateUser(string id, UserUpdateRequest userUpdateRequest)
    {
        var user = await _userRepository.GetUser(id);

        if (user is null)
            return new NotFound("User does not exist!");

        // check if email is not already taken
        if (user.Email != userUpdateRequest.Email && _userRepository.Exists<AppUser>(u => u.Email == user.Email))
            return new BadRequest($"Email '{userUpdateRequest.Email}' is already taken");

        // update user
        user.UserName = userUpdateRequest.UserName;
        user.Email = userUpdateRequest.Email;
        user.UpdatedOn = DateTime.UtcNow;
        if (!await _userRepository.SaveChangesAsync())
            return new Error("User changes could not be persisted!");

        return mapper.Map<AppUser>(user);
    }

    public async Task<OneOf<Success, NotFound, Error>> UploadUserAvatar(Stream sourceStream, string untrustedFileName, string id)
    {
        var user = await _userRepository.GetUser(id);
        if (user is null)
            return new NotFound("User does not exist!");

        try
        {
            var proposedFileExtension = Path.GetExtension(untrustedFileName);
            fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

            // delete old avatar (if any) to avoid file clutter
            var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.UserAvatarsFolder);
            if (user.ProfileImageUri.HasValue())
                fileService.DeleteExistingFile(Path.Combine(physicalRoot, user.ProfileImageUri!));

            // save new avatar
            var trustedFileName = Guid.NewGuid() + proposedFileExtension;
            await fileService.SaveFileToDisk(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

            user.ProfileImageUri = Path.Combine(_appSettings.UserAvatarsFolder, trustedFileName);
            user.UpdatedOn = DateTime.UtcNow;

            return !await _userRepository.SaveChangesAsync()
                ? new Error("User changes could not be persisted!")
                : new Success();
        }
        catch (IOException e)
        {
            return new Error(e.Message);
        }
    }

    public async Task<OneOf<Success, NotFound, Error>> DeleteUser(string id)
    {
        var user = await _userRepository.GetUser(id);
        if (user is null)
            return new NotFound("User does not exist!");

        _userRepository.Delete(user);
        return !await _userRepository.SaveChangesAsync()
            ? new Error("User could not be deleted!")
            : new Success();
    }
}
