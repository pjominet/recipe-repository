using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Data.Entities.Nomenclature;

namespace RecipeRepository.Data.Repositories;

public class QuantityRepository(RecipeRepoContext context) : BaseRepository<RecipeRepoContext>(context)
{
    public async Task<IList<QuantityUnit>> GetQunatityUnits()
    {
        return await Context.QuantityUnits.ToListAsync();
    }
}
