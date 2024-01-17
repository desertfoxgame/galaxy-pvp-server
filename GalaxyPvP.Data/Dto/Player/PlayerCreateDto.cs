using System.ComponentModel.DataAnnotations;

namespace GalaxyPvP.Data
{
    public class PlayerCreateDto
    {
        [Required]
        public string UserId { get; set; }
        public string? Nickname { get; set; }
        public string? PlayfabId { get; set; }
    }
}
