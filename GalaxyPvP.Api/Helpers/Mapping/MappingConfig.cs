﻿using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Api.Helpers.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<GalaxyUser, UserDTO>().ReverseMap();

            CreateMap<Player, PlayerDto>().ReverseMap();
            CreateMap<Player, PlayerCreateDto>().ReverseMap();
            CreateMap<Player, PlayerUpdateDto>().ReverseMap();
            CreateMap<MigrateUserRequestDTO, GalaxyUser>().ReverseMap();
            CreateMap<MigrateUserRequestDTO, Player>().ReverseMap();
            CreateMap<MigrateUserRequestDTO, RegisterRequestDTO>().ReverseMap();
        }

    }
}
