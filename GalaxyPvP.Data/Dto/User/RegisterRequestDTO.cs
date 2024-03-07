namespace GalaxyPvP.Data.Dto.User { 
    public class RegisterRequestDTO
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PlayfabId { get; set; }
        public string? WalletAddress { get; set; }
    }
}
