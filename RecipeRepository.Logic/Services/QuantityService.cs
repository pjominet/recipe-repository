using AutoMapper;
using RecipeRepository.Data.Repositories;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Nomenclature;
using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Logic.Services;

public class QuantityService(RRContext context, IMapper mapper) : IQuantityService
{

    private readonly QuantityRepository _quantityRepository = new(context);

    public async Task<IEnumerable<QuantityUnit>> GetQuantityUnitsAsync()
        => mapper.Map<IEnumerable<QuantityUnit>>(await _quantityRepository.GetQunatityUnits());
}
