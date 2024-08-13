using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Logic.Interfaces;

public interface IUserService
{
    public Task<IEnumerable<AppUser>> GetUsersAsync();
    public Task<AppUser> GetUserAsync(int id);
    public Task<AppUser> UpdateAsync(int id, UserUpdateRequest userUpdateRequest);
    public Task<AppUser> UpdateAsync(int id, RoleUpdateRequest roleUpdateRequest);
    public Task<bool> UploadUserAvatar(Stream sourceStream, string untrustedFileName, int id);
    public Task<bool> Delete(int id);

}
