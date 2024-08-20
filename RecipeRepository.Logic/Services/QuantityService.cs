using AutoMapper;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Data.Repositories;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Nomenclature;

namespace RecipeRepository.Logic.Services;

public class QuantityService(RecipeRepoContext context, IMapper mapper) : IQuantityService
{

    private readonly QuantityRepository _quantityRepository = new(context);

    public async Task<IEnumerable<QuantityUnit>> GetQuantityUnitsAsync()
        => mapper.Map<IEnumerable<QuantityUnit>>(await _quantityRepository.GetQunatityUnits());
}
