using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data.Dto.Player
{
    public class ListPlayerItemDto
    {
        public string PlayerId { get; set; }
        public List<PlayerItem>? PlayerItems { get; set; }
    }
}
