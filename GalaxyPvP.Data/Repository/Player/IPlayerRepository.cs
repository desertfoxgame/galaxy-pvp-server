using Microsoft.EntityFrameworkCore;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IPlayerRepository: IGenericRepository<Player>
    {
        Task<ApiResponse<PlayerDto>> GetByUserId(string userId);
        Task<ApiResponse<PlayerDto>> GetByPlayerId(string playerId);
        Task<ApiResponse<PlayerDto>> Create(PlayerCreateDto playerCreateDto);
        Task<ApiResponse<PlayerUpdateDto>> Update(PlayerUpdateDto playerUpdateDto);
        Task<ApiResponse<PlayerDto>> Delete(string playerId);
        Task<ApiResponse<int>> GetLeaderboard(string playerId);

    }
}