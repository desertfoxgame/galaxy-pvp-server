using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerPostGameStatsDto
    {
        public string MatchId { get; set; }
        public string PlayfabId { get; set; }
        public string DisplayName { get; set; }
        public int WinTeam { get; set; }
        public List<PlayerStats> GameStats { get; set; }

        public bool Equals(PlayerPostGameStatsDto obj)
        {
            if (WinTeam != obj.WinTeam || GameStats?.Count != obj.GameStats?.Count)
                return false;
            for (int i = 0; i < GameStats?.Count; i++)
            {
                if (GameStats[i].Equals(obj.GameStats[i]) == false) return false;
            }
            return true;
        }

        public int GetPlayerEndGame()
        {
            int count = 0;
            for (int i = 0; i < GameStats?.Count; i++)
            {
                if (GameStats[i].isDisconnected == false && 
                    GameStats[i].userId != string.Empty)
                    count++;
            }
            return count;
        }

        internal List<Quantum.PlayerStats> GetQuantumPlayerStats()
        {
            List<Quantum.PlayerStats> result = [];
            for (int i = 0; i < GameStats?.Count; i++)
            {
                Quantum.PlayerStats playerStats = new()
                {
                    NickName = GameStats[i].NickName,
                    TeamID = GameStats[i].TeamID,
                    KDA = GameStats[i].KDA,
                    isDisconnected = GameStats[i].isDisconnected,
                    isBot = GameStats[i].isBot,
                    userId = GameStats[i].userId,
                    tokenInfo = GetTokenInfo(GameStats[i]),
                };
                result.Add(playerStats);
            }
            return result;
        }

        private List<Quantum.TokenInfo> GetTokenInfo(PlayerStats playerStats)
        {
            List<Quantum.TokenInfo> tokenInfos = new();
            for (int i = 0; i < playerStats.TokenInfos?.Count; i++)
            {
                TokenInfo tokenInfo = playerStats.TokenInfos[i];
                Quantum.TokenInfo token = new()
                {
                    tokenId = tokenInfo.tokenId,
                    tokenAddress = tokenInfo.tokenAddress,
                    tokenType = tokenInfo.tokenType,
                    isRented = tokenInfo.isRented,
                };
                tokenInfos.Add(token);
            }
            return tokenInfos;
        }
    }
}
