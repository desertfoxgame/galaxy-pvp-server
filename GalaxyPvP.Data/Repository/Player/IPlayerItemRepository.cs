using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IPlayerItemRepository : IGenericRepository<PlayerItem>
    {
        Task<ApiResponse<PlayerItemDto>> Get(int itemId);
        Task<ApiResponse<ListPlayerItemDto>> GetAll(string playerId);
        Task<ApiResponse<ListPlayerItemDto>> GetAllByNFT(string nftId);
        Task<ApiResponse<PlayerItemDto>> Create(PlayerItemCreateDto playerCreateDto);
        Task<ApiResponse<ListPlayerItemDto>> CreateList(ListPlayerItemDto playerCreateListDto);
        Task<ApiResponse<PlayerItemDto>> Update(PlayerItemDto playerUpdateDto);
        Task<ApiResponse<ListPlayerItemDto>> UpdateList(ListPlayerItemDto playerUpdateDto);
        Task<ApiResponse<PlayerItemDto>> Delete(int playerId);
        Task<ApiResponse<ListPlayerItemDto>> ValidateWallet(ListPlayerItemDto playerCreateListDto);

    }
}
