﻿using System.ComponentModel.DataAnnotations;

namespace GalaxyPvP.Data
{
    public class PlayerCreateDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public string? Nickname { get; set; }
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
