using GalaxyPvP.Data.Dto.Game;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IGameConfigRepository : IGenericRepository<GameConfig>
    {
        Task<ApiResponse<List<GameConfigDTO>>> GetConfigs();
    }
}