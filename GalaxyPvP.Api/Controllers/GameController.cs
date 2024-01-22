using GalaxyPvP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : BaseController
    {
        private readonly IGameConfigRepository _dbGameConfig;
        public GameController(IGameConfigRepository dbGameConfig)
        {
            _dbGameConfig = dbGameConfig;
        }

        [HttpGet("GameConfigs")]
        public async Task<IActionResult> GetGameConfigs()
        {
            var result = await _dbGameConfig.GetConfigs();
            return ReturnFormatedResponse(result);
        }
    }
}
