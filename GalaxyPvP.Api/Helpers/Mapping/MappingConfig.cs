using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Api.Helpers.Mapping
{
    public class MappingConfig:Profile
    {
        public MappingConfig() { 
            CreateMap<GalaxyUser, UserDTO>().ReverseMap();

            CreateMap<Player, PlayerDto>().ReverseMap();
            CreateMap<Player, PlayerCreateDto>().ReverseMap();
            CreateMap<Player, PlayerUpdateDto>().ReverseMap();
        }

    }
}
