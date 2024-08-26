using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Logic.Interfaces;

public interface IUserService
{
    public Task<IEnumerable<AppUser>> GetUsers();
    public Task<AppUser> GetUser(string id);
    public Task<AppUser> Update(string id, UserUpdateRequest userUpdateRequest);
    public Task<bool> UploadUserAvatar(Stream sourceStream, string untrustedFileName, string id);
    public Task<bool> Delete(string id);
}
