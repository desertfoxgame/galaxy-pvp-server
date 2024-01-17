using GalaxyPvP.Data.DTO;

namespace GalaxyPvP.Data.Dto.User
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
