using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IPlayerItemRepository : IGenericRepository<PlayerItem>
    {
        Task<ApiResponse<PlayerItemDto>> Get(int itemId, string userId);
        Task<ApiResponse<ListPlayerItemDto>> GetAll(string userId);
        Task<ApiResponse<ListPlayerItemDto>> GetAllByNFT(string nftId);
        Task<ApiResponse<PlayerItemDto>> Create(string userId, PlayerItemCreateDto playerCreateDto);
        Task<ApiResponse<ListCreatePlayerItemDto>> CreateList(string userId, ListCreatePlayerItemDto playerCreateListDto);
        Task<ApiResponse<PlayerItemDto>> Update(string userId, PlayerItemUpdateDto playerUpdateDto);
        Task<ApiResponse<ListUpdatePlayerItemDto>> UpdateList(string userId, ListUpdatePlayerItemDto playerUpdateDto);
        Task<ApiResponse<PlayerItemDto>> Delete(string userId, int dataId);
        Task<ApiResponse<ListPlayerItemDto>> ValidateWallet(ListPlayerItemDto playerCreateListDto);

    }
}
