﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyPvP.Data.Model
{
    public class Player:BaseModel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [ForeignKey("User")]
        [MaxLength(450)]
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
    }
}
