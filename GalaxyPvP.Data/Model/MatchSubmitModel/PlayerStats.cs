using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerStats
    {
        public string NickName { get; set; }

        public int TeamID { get; set; }

        public string KDA { get; set; }

        public bool isDisconnected { get; set; }

        public bool isBot { get; set; }

        public string userId { get; set; }

        public List<TokenInfo>? TokenInfos { get; set; }

        public bool Equals(PlayerStats obj)
        {
            if (NickName != obj.NickName || TeamID != obj.TeamID
                || KDA != obj.KDA || isDisconnected != obj.isDisconnected
                || isBot != obj.isBot || userId != obj.userId
                || TokenInfos?.Count != obj.TokenInfos?.Count) 
                return false;
            for (int i = 0; i < TokenInfos?.Count; i++)
            {
                if (TokenInfos[i].Equals(obj.TokenInfos[i]) == false) return false;
            }
            return true;
        }
    }

    public class HeroStatsData
    {
        public string playerId;
        public int Kills;
        public int Deaths;
        public int Assits;
        public int Score => (Kills * 2 + Assits) - Deaths;

        public HeroStatsData(string playerId, int kills, int deaths, int assit)
        {
            this.playerId = playerId;
            this.Kills = kills;
            this.Deaths = deaths;
            this.Assits = assit;
        }
    }
}
