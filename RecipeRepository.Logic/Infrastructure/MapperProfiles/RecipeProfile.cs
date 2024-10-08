﻿using AutoMapper;
using Entities = RecipeRepository.Data.Entities;

namespace RecipeRepository.Logic.Infrastructure.MapperProfiles;

public class RecipeProfile : Profile
{
        public RecipeProfile()
        {
            CreateMap<Entities.Recipe, Models.Recipe>()
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.RecipeTags.Select(rta => rta.Tag)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.CostId))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.DifficultyId))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.RecipeLikes.Select(rl => rl.UserId)))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null));
        }
}
