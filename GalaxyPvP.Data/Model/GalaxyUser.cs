using Microsoft.AspNetCore.Identity;

namespace GalaxyPvP.Data
{
    public class GalaxyUser:IdentityUser
    {
        public string? PlayfabId { get; set; }
        public string? WalletAddress {  get; set; }
        public string? Token {  get; set; }
    }
}
