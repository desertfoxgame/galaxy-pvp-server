using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data.Dto.Player
{
    public class ListUpdatePlayerItemDto
    {
        public string PlayerId { get; set; }
        public List<PlayerItemUpdateDto>? PlayerItems { get; set; }
    }
}
