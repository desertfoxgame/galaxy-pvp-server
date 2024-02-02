
using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.MatchSubmit;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Data.Repository.MatchMaking;
using GalaxyPvP.Extensions;
using GalaxyPvP.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.RegularExpressions;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchSubmitController : BaseController
    {
        //private readonly Dictionary<string, PlayerMatchInfo> matchSubmit = new ();
        private readonly IMapper _mapper;
        private readonly IMatchResultRepository _resultRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMemoryCache _memoryCache;

        public MatchSubmitController(IMapper mapper, IMemoryCache memoryCache, IMatchResultRepository resultRepository, IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _memoryCache = memoryCache;
            _resultRepository = resultRepository;
            _playerRepository = playerRepository;
        }
        [HttpPost("RegisterMatch")]
        public async Task<IActionResult> RegisterMatch([FromBody] PlayerRegisterMatchDto dto)
        {
            Dictionary<string, PlayerMatchInfo> matchSubmit = GetMatchSubmitDic();
            var player = _mapper.Map<PlayerRegisterMatchDto>(dto);
            if (GetMatchSubmitDic().TryGetValue(player.matchId, out PlayerMatchInfo info))
            {
                info.AddPlayer(player);
                matchSubmit[player.matchId] = info;
            } else
            {
                matchSubmit.Add(player.matchId, new PlayerMatchInfo(player));
            }

            SaveMatchSubmitDic(matchSubmit);
            return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Success"));
        }
        //

        // id, matchid
        // call server nft game matchid win or lose, get keyfragment
        // win -> updateUserdata: totalgame, trophy, ...wingamestreak
        [HttpPost("GetMatchStage")]
        public async Task<IActionResult> GetMatchStage(string matchId)
        {
            var client = new HttpClient();
            string url = "https://gfc-game.azurewebsites.net/Match/state/" + matchId;
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                // Read the content as a string
                string data = await response.Content.ReadAsStringAsync();
                return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200(data));
            }
            else
            {
                return ReturnFormatedResponse(ApiResponse<string>.Return404("Server Error"));
            }
        }

        [HttpPost("MatchResult")]
        public async Task<IActionResult> MatchResult([FromBody] PlayerPostGameStatsDto dto)
        {
            Dictionary<string, PlayerMatchInfo> matchSubmit = GetMatchSubmitDic();

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
                        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                        string apiKey = config["SubmitKey:ApiKey"];
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                        GameRewardAPI _api = new("https://gfc-game.azurewebsites.net/", client);
                        try
                        {
                            await _api.HistoryAsync(model, model.MatchId);
                            info.isSubmitMatchResult = true;

                            // Update userData
                            Dictionary<string, bool> users = info.GetUsers();
                            foreach (var user in users)
                            {
                                await UpdateUserData(user.Key, user.Value);
                            }
                            // Add MatchResult to table
                            MatchResultDto matchResultDto = new ();
                            MatchDataDto matchDataDto = new(info.matchType, result.WinTeam, result.GameStats);
                            matchResultDto.MatchId = result.MatchId;
                            matchResultDto.MatchData = matchDataDto;
                            await _resultRepository.Create(matchResultDto);


                        } catch (Exception ex)
                        {
                            ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, $"History Async error {ex.Message}"));
                        }
                    }
                    matchSubmit[result.MatchId] = info;
                    SaveMatchSubmitDic(matchSubmit);

                    return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Success"));
                }
                else
                    return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "The player is not in the match"));
            } else
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "Match Not Found"));
            }
        }

        private async Task UpdateUserData(string userId, bool isWon)
        {
            // update user data here
            // using GalaxyExtensions.GetRewardTrophy go get reward trophy
            int inputTrophy = _playerRepository.GetByUserId(userId).Result.Data.Trophy;
            await _playerRepository.UpdatePlayerTrophyByUserId(userId, GalaxyExtensions.GetRewardTrophy(inputTrophy, isWon));
        }

        Dictionary<string, PlayerMatchInfo> GetMatchSubmitDic()
        {
            // Try to get the dictionary from the cache
            if (_memoryCache.TryGetValue("MatchSubmit", out Dictionary<string, PlayerMatchInfo> cachedData))
            {
                return cachedData;
            }

            // If not in the cache, return null or fetch it from the data source
            return new Dictionary<string, PlayerMatchInfo>();
        }

        void SaveMatchSubmitDic(Dictionary<string, PlayerMatchInfo> data)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Set expiration time to 10 minutes
            };
            // Set the dictionary in the cache with a specific key and expiration time
            _memoryCache.Set("MatchSubmit", data);
        }
    }
}
