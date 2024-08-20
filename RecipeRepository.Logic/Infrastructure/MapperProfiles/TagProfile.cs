using AutoMapper;
using Entities = RecipeRepository.Data.Entities;

namespace RecipeRepository.Logic.Infrastructure.MapperProfiles;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<Entities.Nomenclature.Tag, Models.Nomenclature.Tag>()
            .ForMember(dest => dest.Recipes, opt =>
                opt.MapFrom(src => src.RecipeTags.Select(rta => rta.Recipe)));

        CreateMap<Models.Nomenclature.Tag, Entities.Nomenclature.Tag>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TagCategory, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeTags, opt => opt.Ignore());

        CreateMap<Entities.Nomenclature.TagCategory, Models.Nomenclature.TagCategory>();
        CreateMap<Models.Nomenclature.TagCategory, Entities.Nomenclature.TagCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore());
    }
}
