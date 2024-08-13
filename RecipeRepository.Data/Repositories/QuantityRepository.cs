using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Nomenclature;
using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Data.Repositories;

public class QuantityRepository(RRContext context) : BaseRepository<RRContext>(context)
{
    public async Task<IList<QuantityUnit>> GetQunatityUnits()
    {
        return await Context.QuantityUnits.ToListAsync();
    }
}
