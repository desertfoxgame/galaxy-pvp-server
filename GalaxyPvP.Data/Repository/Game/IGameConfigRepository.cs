using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IGameConfigRepository:IGenericRepository<GameConfig>
    {
        Task<ApiResponse<string>> GetConfigs();
    }
}