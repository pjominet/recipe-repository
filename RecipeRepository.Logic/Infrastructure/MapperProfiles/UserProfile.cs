using AutoMapper;
using Entities = RecipeRepository.Data.Entities;

namespace RecipeRepository.Logic.Infrastructure.MapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
        {
            CreateMap<Entities.Identity.AppUser, Models.Identity.AppUser>()
                .ForMember(dest => dest.LikedRecipes, opt =>
                    opt.MapFrom(src => src.RecipeLikes.Select(rl => rl.Recipe)));

            CreateMap<Models.Identity.AppUser, Entities.Identity.AppUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.Recipes, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeLikes, opt => opt.Ignore());

            CreateMap<Models.Identity.RegisterRequest, Entities.Identity.AppUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageUri, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.Recipes, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeLikes, opt => opt.Ignore());
        }
}
