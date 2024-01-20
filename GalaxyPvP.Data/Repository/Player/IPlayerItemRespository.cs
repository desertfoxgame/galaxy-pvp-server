using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IPlayerItemRespository : IGenericRepository<PlayerItem>
    {
        Task<ApiResponse<PlayerItemDto>> Get(int itemId);
        Task<ApiResponse<ListPlayerItemDto>> GetAll(string playerId);
        Task<ApiResponse<PlayerItemDto>> Create(PlayerItemCreateDto playerCreateDto);
        Task<ApiResponse<PlayerItemDto>> Update(PlayerItemDto playerUpdateDto);
        Task<ApiResponse<PlayerItemDto>> Delete(int playerId);
    }
}
