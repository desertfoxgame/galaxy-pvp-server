using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerUpdateDto
    {
        public string? Nickname { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Trophy { get; set; }
        public int Win { get; set; }
        public int TotalGames { get; set; }
        public int WinStreak { get; set; }
        public int WinStreakCurrent { get; set; }
        public int MVP { get; set; }
        public long banTime { get; set; }

        public PlayerUpdateDto(string nickname, int Level, int Exp, int Trophy, int Win, int TotalGames, int WinStreak, int WinStreakCurrent, int Mvp) 
        {
            this.Nickname = nickname;
            this.Level = Level;
            this.Exp = Exp;
            this.Trophy = Trophy;
            this.Win = Win;
            this.TotalGames = TotalGames;
            this.WinStreak = WinStreak;
            this.WinStreakCurrent = WinStreakCurrent;
            this.MVP = Mvp;
        }
    }
}
