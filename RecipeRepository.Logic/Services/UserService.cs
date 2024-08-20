using AutoMapper;
using Microsoft.Extensions.Options;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Data.Repositories;
using RecipeRepository.Logic.Infrastructure.Extensions;
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

    public async Task<AppUser> GetUser(string id)
    {
        return mapper.Map<AppUser>(await _userRepository.GetUser(id));
    }

    public async Task<AppUser> Update(string id, UserUpdateRequest userUpdateRequest)
    {
        var user = await _userRepository.GetUser(id);

        if (user is null)
            throw new KeyNotFoundException("AppUser not found");

        // check if email is not already taken
        if (user.Email != userUpdateRequest.Email && _userRepository.Exists<AppUser>(u => u.Email == user.Email))
            throw new BadRequestException($"Email '{userUpdateRequest.Email}' is already taken");

        // update user
        user.UserName = userUpdateRequest.UserName;
        user.Email = userUpdateRequest.Email;
        user.UpdatedOn = DateTime.UtcNow;
        if (!await _userRepository.SaveChangesAsync())
            throw new ApplicationException("Database error: Changes could not be saved correctly");

        return mapper.Map<AppUser>(user);
    }

    public async Task<AppUser> Update(string id, RoleUpdateRequest roleUpdateRequest)
    {
        var user = await _userRepository.GetUser(id);

        if (user is null)
            throw new KeyNotFoundException("AppUser not found");

        // update role
        user.UpdatedOn = DateTime.UtcNow;
        if (!await _userRepository.SaveChangesAsync())
            throw new ApplicationException("Database error: Changes could not be saved correctly");

        return mapper.Map<AppUser>(user);
    }

    public async Task<bool> UploadUserAvatar(Stream sourceStream, string untrustedFileName, string id)
    {
        var user = await _userRepository.GetUser(id);
        if (user is null)
            throw new KeyNotFoundException("AppUser to add avatar to could not be found");

        try
        {
            var proposedFileExtension = Path.GetExtension(untrustedFileName);
            fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

            // delete old avatar (if any) to avoid file clutter
            var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/profiles");
            if (user.ProfileImageUri.HasValue())
                fileService.DeleteExistingFile(Path.Combine(physicalRoot, user.ProfileImageUri!));

            // save new avatar
            var trustedFileName = Guid.NewGuid() + proposedFileExtension;
            await fileService.SaveFileToDisk(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

            user.ProfileImageUri = Path.Combine(_appSettings.UserAvatarsFolder, trustedFileName);
            user.UpdatedOn = DateTime.UtcNow;

            return await _userRepository.SaveChangesAsync();
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
            throw new BadRequestException(e.Message);
        }
    }

    public async Task<bool> Delete(string id)
    {
        var user = await _userRepository.GetUser(id);
        if (user is null)
            throw new KeyNotFoundException("AppUser to delete could not be found.");

        _userRepository.Delete(user);
        return await _userRepository.SaveChangesAsync();
    }
}
