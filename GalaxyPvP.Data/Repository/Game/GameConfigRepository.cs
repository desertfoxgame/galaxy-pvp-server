using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GalaxyPvP.Data
{
    public class GameConfigRepository : GenericRepository<GameConfig, GalaxyPvPContext>, IGameConfigRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public GameConfigRepository(GalaxyPvPContext context, IMapper mapper) : base(context)
        {
            _db = context;
            _mapper = mapper;
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
