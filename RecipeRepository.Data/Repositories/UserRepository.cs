using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Identity;
using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Data.Repositories;

public class UserRepository(RRContext context) : BaseRepository<RRContext>(context)
{
    public async Task<IEnumerable<AppUser>> GetUsers() => await Context.AppUsers.ToListAsync();

    public async Task<AppUser?> GetUser(string id) => await Context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
}
