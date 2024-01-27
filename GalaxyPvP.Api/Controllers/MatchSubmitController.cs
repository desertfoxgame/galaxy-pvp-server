
using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Extensions;
using GalaxyPvP.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchSubmitController : BaseController
    {
        private readonly Dictionary<string, PlayerMatchInfo> matchSubmit = new ();
        private readonly IMapper _mapper;
        public MatchSubmitController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [HttpPost("RegisterMatch")]
        public async Task<IActionResult> RegisterMatch([FromBody] PlayerRegisterMatchDto dto)
        {
            var player = _mapper.Map<PlayerRegisterMatchDto>(dto);
            if (matchSubmit.TryGetValue(player.matchId, out PlayerMatchInfo info))
            {
                info.AddPlayer(player);
                matchSubmit[player.matchId] = info;
            } else
            {
                matchSubmit.Add(player.matchId, new PlayerMatchInfo(player));
            }
            return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Success"));
        }

        [HttpPost("MatchResult")]
        public async Task<IActionResult> MatchResult([FromBody] PlayerPostGameStatsDto dto)
        {
            var result = _mapper.Map<PlayerPostGameStatsDto>(dto);
            if (matchSubmit.TryGetValue(result.MatchId, out PlayerMatchInfo info))
            {
                if (info.AddMatchResult(result))
                {
                    if (info.IsSubmidMatch())
                    {
                        var model = new GetRewardRequest
                        {
                            MatchId = result.MatchId,
                            Results = info.ConvertGameStatsToCommand(),
                            Sessions = info.sessionInfos.ToArray(),
                        };
                        var client = new HttpClient();
                        string apiKey = "729FA88E-152A-458E-B8D5-EC1CABA7DD81";
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                        GameRewardAPI _api = new("https://gfc-game.azurewebsites.net/", client);
                        await _api.HistoryAsync(model, model.MatchId);
                        info.isSubmitMatchResult = true;
                    }
                    matchSubmit[result.MatchId] = info;
                    return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Success"));
                }
                else
                    return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "The player is not in the match"));
            } else
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "Match Not Found"));
            }
        }
    }
}
