using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.Player;
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

            //Player
            CreateMap<Player, PlayerDto>().ReverseMap();
            CreateMap<Player, PlayerCreateDto>().ReverseMap();
            CreateMap<Player, PlayerUpdateDto>().ReverseMap();

            //Migrate User
            CreateMap<MigrateUserRequestDTO, GalaxyUser>().ReverseMap();
            CreateMap<MigrateUserRequestDTO, Player>().ReverseMap();
            CreateMap<MigrateUserRequestDTO, RegisterRequestDTO>().ReverseMap();

            //Player Item
            CreateMap<PlayerItem, PlayerItemDto>().ReverseMap();
            CreateMap<PlayerItemCreateDto, PlayerItemDto>().ReverseMap();
            CreateMap<PlayerItemCreateDto, PlayerItem>().ReverseMap();
            CreateMap<PlayerItemUpdateDto, PlayerItem>().ReverseMap();
        }

    }
}
