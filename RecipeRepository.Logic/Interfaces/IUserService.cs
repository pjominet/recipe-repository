using OneOf;
using RecipeRepository.Logic.Infrastructure.OneOfResults;
using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Logic.Interfaces;

public interface IUserService
{
    public Task<IEnumerable<AppUser>> GetUsers();
    public Task<AppUser?> GetUser(string id);
    public Task<OneOf<AppUser, BadRequest, NotFound, Error>> UpdateUser(string id, UserUpdateRequest userUpdateRequest);
    public Task<OneOf<Success, NotFound, Error>> UploadUserAvatar(Stream sourceStream, string untrustedFileName, string id);
    public Task<OneOf<Success, NotFound, Error>> DeleteUser(string id);
}
