using Microsoft.EntityFrameworkCore;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IPlayerRepository: IGenericRepository<Player>
    {
        Task<ApiResponse<PlayerDto>> Get(string userId);
        Task<ApiResponse<PlayerDto>> Create(PlayerCreateDto playerCreateDto);
        Task<ApiResponse<PlayerDto>> Update(PlayerDto playerUpdateDto);
        Task<ApiResponse<PlayerDto>> Delete(int playerId);

    }
}