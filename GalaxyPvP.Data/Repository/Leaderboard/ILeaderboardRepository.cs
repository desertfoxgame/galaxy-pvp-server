using GalaxyPvP.Data.Dto.Leaderboard;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface ILeaderboardRepository : IGenericRepository<Leaderboard>
    {
        Task<ApiResponse<ListLeaderboardDTO>> GetTopList(int amount);
        Task<ApiResponse<ListLeaderboardDTO>> Update();
        Task<ApiResponse<LeaderboardDTO>> GetByRank(int rank);
        Task<ApiResponse<LeaderboardDTO>> GetByPlayerId(string playerId);
        Task<ApiResponse<LeaderboardDTO>> Create(PlayerDto playerDto);
        Task<ApiResponse<string>> AddAllPlayer();
        Task<ApiResponse<string>> AddPlayer(string playerId);
    }
}
