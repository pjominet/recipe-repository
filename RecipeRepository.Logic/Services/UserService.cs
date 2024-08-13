using AutoMapper;
using Microsoft.Extensions.Options;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRepository.Data.Repositories;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Identity;
using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Logic.Services
{
    public class UserService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings, IFileService fileService)
        : IUserService
    {
        private readonly UserRepository _userRepository = new(context);
        private readonly AppSettings _appSettings = appSettings.Value;

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return mapper.Map<IEnumerable<AppUser>>(await _userRepository.GetAllAsync<AppUser>(null, $"{nameof(AppUser.Role)}"));
        }

        public async Task<AppUser> GetUserAsync(int id)
        {
            return mapper.Map<AppUser>(await _userRepository.GetFirstOrDefaultAsync<AppUser>(u => u.Id == id, $"{nameof(AppUser.Role)}"));
        }

        public async Task<AppUser> UpdateAsync(int id, UserUpdateRequest userUpdateRequest)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync<AppUser>(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("AppUser not found");

            // check if email is not already taken
            if (user.Email != userUpdateRequest.Email && _userRepository.Exists<AppUser>(u => u.Email == user.Email))
                throw new BadRequestException($"Email '{userUpdateRequest.Email}' is already taken");

            // update user
            user.Username = userUpdateRequest.UserName;
            user.Email = userUpdateRequest.Email;
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            if (!await _userRepository.SaveChangesAsync())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            return mapper.Map<AppUser>(user);
        }

        public async Task<AppUser> UpdateAsync(int id, RoleUpdateRequest roleUpdateRequest)
        {
            if (await _userRepository.AdminCountAsync() <= 1)
                throw new BadRequestException("The last admin can't demote himself!");

            var user = await _userRepository.GetFirstOrDefaultAsync<AppUser>(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("AppUser not found");

            // update role
            user.RoleId = (int) roleUpdateRequest.AppRole;
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            if (!await _userRepository.SaveChangesAsync())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            return mapper.Map<AppUser>(user);
        }

        public async Task<bool> UploadUserAvatar(Stream sourceStream, string untrustedFileName, int id)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync<AppUser>(u => u.Id == id);
            if (user == null)
                throw new KeyNotFoundException("AppUser to add avatar to could not be found");

            try
            {
                var proposedFileExtension = Path.GetExtension(untrustedFileName);
                 fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

                 // delete old avatar (if any) to avoid file clutter
                 var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot");
                 if (!string.IsNullOrWhiteSpace(user.ProfileImageUri))
                    fileService.DeleteExistingFile(Path.Combine(physicalRoot, user.ProfileImageUri));

                 // save new avatar
                var trustedFileName = Guid.NewGuid() + proposedFileExtension;
                await fileService.SaveFileToDiskAsync(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

                user.ProfileImageUri = Path.Combine(_appSettings.UserAvatarsFolder, trustedFileName);
                user.UpdatedOn = DateTime.UtcNow;
                _userRepository.Update(user);

                return await _userRepository.SaveChangesAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw new BadRequestException(e.Message);
            }
        }

        public async Task<bool> Delete(int id)
        {
            if (await _userRepository.AdminCountAsync() <= 1)
                throw new BadRequestException("The last admin can't delete his account!");

            var user = await _userRepository.GetFirstOrDefaultAsync<AppUser>(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("AppUser to delete could not be found.");

            _userRepository.Delete(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> ToggleUserLock(int id, LockRequest lockRequest)
        {
            if (lockRequest.LockedById.HasValue && lockRequest.LockedById == id)
                throw new BadRequestException("Locking yourself is not allowed!");

            var user = await _userRepository.GetFirstOrDefaultAsync<AppUser>(u => u.Id == id);

            user.LockedOn = lockRequest.UserLock
                ? (DateTime?) DateTime.UtcNow
                : null;

            user.LockedById = lockRequest.LockedById;

            _userRepository.Update(user);
            return await _userRepository.SaveChangesAsync();
        }
    }
}
