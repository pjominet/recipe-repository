using RecipeRepository.Logic.Models.Nomenclature;

namespace RecipeRepository.Logic.Interfaces;

public interface IQuantityService
{
    public Task<IEnumerable<QuantityUnit>> GetQuantityUnitsAsync();
}
