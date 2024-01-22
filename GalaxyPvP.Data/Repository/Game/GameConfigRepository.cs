using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Game;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        public async Task<ApiResponse<List<GameConfigDTO>>> GetConfigs()
        {
            try
            {
                List<GameConfig> configs = await All.ToListAsync();
               
                return ApiResponse<List<GameConfigDTO>>.ReturnResultWith200(_mapper.Map<List<GameConfigDTO>>(configs));         
            }
            catch (Exception ex)
            {
                return ApiResponse<List<GameConfigDTO>>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
