using System.ComponentModel.DataAnnotations;

namespace GalaxyPvP.Data
{
    public class PlayerCreateDto
    {
        public string? Id { get; set; }
        public string UserId { get; set; }
        public string? Nickname { get; set; }
        public string? EquipData { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Trophy { get; set; }
        public int Win { get; set; }
        public int TotalGames { get; set; }
        public int WinStreak { get; set; }
        public int WinStreakCurrent { get; set; }
        public int MVP { get; set; }
        public short Tutorial { get; set; }
        public short isAdmin { get; set; }
        public short Developer { get; set; }
    }
}
