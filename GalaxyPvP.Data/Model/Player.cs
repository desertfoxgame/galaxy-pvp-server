using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyPvP.Data.Model
{
    public class Player:BaseModel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(450)]
        [ForeignKey("User")] // Specify the navigation property name
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Trophy { get; set; }
        public int Win { get; set; }
        public int TotalGames { get; set; }
        public int WinStreak { get; set; }
        public int WinStreakCurrent { get; set; }
        public int MVP { get; set; }
        public string? EquipData { get; set; }
        public short Tutorial { get; set; }
        public short isAdmin { get; set; }
        public short Developer { get; set; }
        public bool ConfirmTermsOfService { get; set; }

        public GalaxyUser User { get; set; }
        public ICollection<Friend> FriendsAsPlayer1 { get; set; }
        public ICollection<Friend> FriendsAsPlayer2 { get; set; }
    }
}
