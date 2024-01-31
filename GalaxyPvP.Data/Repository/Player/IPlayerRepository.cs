using Microsoft.EntityFrameworkCore;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Dto.Player;

namespace GalaxyPvP.Data
{
    public interface IPlayerRepository: IGenericRepository<Player>
    {
        Task<ApiResponse<PlayerDto>> GetByUserId(string userId);
        Task<ApiResponse<string>> GetPlayerEquipData(string playerId);
        Task<ApiResponse<PlayerDto>> UpdatePlayerEquipData(string playerId, string equipdata);
        Task<ApiResponse<PlayerDto>> GetByPlayerId(string playerId);
        Task<ApiResponse<PlayerDto>> Create(PlayerCreateDto playerCreateDto);
        Task<ApiResponse<PlayerUpdateDto>> Update(PlayerUpdateDto playerUpdateDto);
        Task<ApiResponse<PlayerDto>> Delete(string playerId);
        Task<ApiResponse<List<PlayerDto>>> GetLeaderboard(int amount);

    }
}