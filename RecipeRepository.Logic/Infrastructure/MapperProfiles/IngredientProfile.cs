using AutoMapper;
using Entities = RecipeRepository.Data.Entities;

namespace RecipeRepository.Logic.Infrastructure.MapperProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Entities.Ingredient, Models.Ingredient>();
        CreateMap<Models.Ingredient, Entities.Ingredient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuantityUnit, opt => opt.Ignore())
            .ForMember(dest => dest.Recipe, opt => opt.Ignore());

        CreateMap<Entities.Nomenclature.QuantityUnit, Models.Nomenclature.QuantityUnit>();
        CreateMap<Models.Nomenclature.QuantityUnit, Entities.Nomenclature.QuantityUnit>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
    }
}
