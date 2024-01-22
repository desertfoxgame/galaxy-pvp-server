using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data.Dto.Player
{
    public class ListCreatePlayerItemDto
    {
        public string PlayerId { get; set; }
        public List<PlayerItemCreateDto>? PlayerItems { get; set; }
    }
}
