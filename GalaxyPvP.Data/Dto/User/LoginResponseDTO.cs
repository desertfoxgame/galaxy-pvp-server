using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data.Dto.User
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public PlayerDto Player { get; set; }
        public List<PlayerItem> Items { get; set; }
    }
}
