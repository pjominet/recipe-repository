using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Data.Entities.Identity;

namespace RecipeRepository.Data.Repositories;

public class UserRepository(RecipeRepoContext context) : BaseRepository<RecipeRepoContext>(context)
{
    public async Task<IEnumerable<AppUser>> GetUsers() => await Context.AppUsers.ToListAsync();

    public async Task<AppUser?> GetUser(string id) => await Context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
}
