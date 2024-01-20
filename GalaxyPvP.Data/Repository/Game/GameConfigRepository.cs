using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GalaxyPvP.Data
{
    public class GameConfigRepository : GenericRepository<GameConfig, GalaxyPvPContext>, IGameConfigRepository
    {
        protected GameConfigRepository(GalaxyPvPContext context) : base(context)
        {
        }

        public async Task<ApiResponse<string>> GetConfigs()
        {
            try
            {
                var configs = await All.ToListAsync();
                string configJson = JsonConvert.SerializeObject(configs);
                return ApiResponse<string>.ReturnResultWith200(configJson);         
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
