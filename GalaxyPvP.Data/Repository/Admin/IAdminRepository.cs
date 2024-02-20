using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IAdminRepository : IGenericRepository<Player>
    {
        Task<ApiResponse<PageResponse<AdminPlayerDTO>>> GetAllPlayer(PageRequest request);
        Task<ApiResponse<PageResponse<AdminPlayerDTO>>> SearchPlayer(PageRequest request, string searchInput);
        Task<ApiResponse<PlayerUpdateDto>> Update(PlayerUpdateDto playerUpdateDto);

    }
}
