using Microsoft.Identity.Client;
using Quantum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerMatchInfo
    {
        public bool isSubmitMatchResult { get; set; }
        public string matchType { get; set; }
        public List<PlayerSessionInfo> sessionInfos { get; set; }
        public List<PlayerPostGameStatsDto>? playerPostGames { get; set; }

        private int submitValid;

        public PlayerMatchInfo(PlayerRegisterMatchDto dto) 
        {
            isSubmitMatchResult = false;
            matchType = dto.matchType;
            sessionInfos = [];
            sessionInfos.Add(new PlayerSessionInfo()
            {
                UserId = dto.playfabId,
                SessionTicket = dto.sessionTicket,
            });
            playerPostGames = [];
            submitValid = -1;
        }

        public void AddPlayer(PlayerRegisterMatchDto dto)
        {
            sessionInfos.Add(new PlayerSessionInfo() { UserId = dto.playfabId, SessionTicket = dto.sessionTicket });
        }

        public bool AddMatchResult(PlayerPostGameStatsDto dto) 
        {
            for (int i = 0; i < sessionInfos?.Count; i++)
            {
                if (sessionInfos[i].UserId == dto.PlayfabId)
                {
                    playerPostGames?.Add(dto);
                    if (submitValid == -1)
                    {
                        submitValid = dto.GetPlayerEndGame();
                    }
                    return true;
                }
            }
            return false;
        }
        public bool IsSubmidMatch()
        {
            if (playerPostGames?.Count >= sessionInfos?.Count || 
                playerPostGames?.Count >= submitValid ||
                playerPostGames?.Count > 2) return true;
            return false;
        }

        public CommandPostGameStats[] ConvertGameStatsToCommand()
        {
            List<CommandPostGameStats> results = [];
            PlayerPostGameStatsDto? dto = playerPostGames?.FirstOrDefault();
            if (dto != null && dto != default)
            {
                for (int i = 0; i < dto.GameStats?.Count; i++)
                {
                    PlayerStats playerStats = dto.GameStats[i];
                    if (playerStats.isDisconnected == false && playerStats.userId != string.Empty)
                    {
                        CommandPostGameStats cmd = new()
                        {
                            match_id = dto.MatchId,
                            match_type = matchType,
                            is_rank_match = true,
                            submit_player = playerStats.NickName,
                            winner_team = dto.WinTeam,
                            playfab_id = playerStats.userId,
                            player_stats = dto.GetQuantumPlayerStats()
                        };
                    }
                }
            }

            return results.ToArray();
        }
    }
}
