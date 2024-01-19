using System.ComponentModel.DataAnnotations;

namespace GalaxyPvP.Data
{
    public class PlayerCreateDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public string? Nickname { get; set; }
    }
}
