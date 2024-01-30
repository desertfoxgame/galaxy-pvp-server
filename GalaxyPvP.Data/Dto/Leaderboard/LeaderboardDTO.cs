using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Dto.Leaderboard
{
    public class LeaderboardDTO
    {
        public string PlayerId { get; set; }
        public string DisplayName { get; set; }
        public int Trophy { get; set; }
        public int Rank { get; set; }
    }
}
