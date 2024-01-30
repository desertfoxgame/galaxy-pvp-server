using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class MatchDataDto
    {
        public string MatchType { get; set; }
        public int WinTeam { get; set; }
        public List<PlayerStats> GameStats { get; set; }

        public MatchDataDto(string matchType, int winTeam, List<PlayerStats> gameStats)
        {
            MatchType = matchType;
            WinTeam = winTeam;
            GameStats = gameStats;
        }
    }
}
