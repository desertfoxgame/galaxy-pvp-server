using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using GalaxyPvP.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GalaxyPvP.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MatchMakingController : BaseController
    {
        private readonly UserManager<GalaxyUser> _userManager;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private IHttpContextAccessor _httpContext;
        private readonly IMemoryCache _memoryCache;

        //private Dictionary<string, MatchMakingTicket> PlayerPools = new Dictionary<string, MatchMakingTicket>();

        public MatchMakingController(UserManager<GalaxyUser> userManager, IPlayerRepository playerManager, IMapper mapper, 
            UserInfoToken userInfoToken,
            IHttpContextAccessor httpContext,
            IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _playerRepository = playerManager;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _httpContext = httpContext;
            _memoryCache = memoryCache;

        }

        [HttpPost("SubmitTicket")]
        [Authorize]
        public async Task<IActionResult> SubmitTicket([FromBody] MatchMakingTicketDTO createDto)
        {
            //string userId = _userManager.GetUserId(_httpContext.HttpContext.User);
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            Serilog.Log.Debug("----------------User ID------ " + userId);
            if(string.IsNullOrEmpty(userId))
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "User not found"));
            }
            var getPlayer = await _playerRepository.GetByUserId(userId);
            if(!getPlayer.Success)
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "Player not found"));
            }
            PlayerDto player = getPlayer.Data;
            Dictionary<string, MatchMakingTicket> PlayerPools = GetCachedDictionary();

            if (PlayerPools == null)
            {
                PlayerPools = new Dictionary<string, MatchMakingTicket>();
                SaveDictionaryToCache(PlayerPools);
            }

            if (PlayerPools.ContainsKey(player.Id))
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "Already in queued"));
            }
            var ticket = _mapper.Map<MatchMakingTicket>(createDto);
            ticket.PlayerId = player.Id;
            ticket.SubmitedTime = DateTime.UtcNow;
            PlayerPools.Add(player.Id, ticket);
            SaveDictionaryToCache(PlayerPools);

            return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Success"));
        }

        [HttpPost("CheckForMatch")]
        [Authorize]
        public async Task<IActionResult> CheckForMatch([FromBody] MatchRequestDto request)
        {
            Dictionary<string, MatchMakingTicket> PlayerPools = GetCachedDictionary();

            if (PlayerPools == null)
            {
                PlayerPools = new Dictionary<string, MatchMakingTicket>();
                SaveDictionaryToCache(PlayerPools);
            }

            if (PlayerPools.ContainsKey(request.PlayerId))
            {
                if(PlayerPools.Count >= 6)
                {
                    return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Game ready!"));
                }
                else if (PlayerPools.Count >= 4 && request.WaitTime >= 30) {
                    return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Game ready with bot!"));
                }

                return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Already in queued"));
            }
            else
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "Player not in queued"));
            }

            //return ReturnFormatedResponse(ApiResponse<string>.ReturnSuccess());
        }

        void SaveDictionaryToCache(Dictionary<string, MatchMakingTicket> data)
        {
            // Set the dictionary in the cache with a specific key and expiration time
            _memoryCache.Set("PlayerPools", data);
        }

        Dictionary<string, MatchMakingTicket> GetCachedDictionary()
        {
            // Try to get the dictionary from the cache
            if (_memoryCache.TryGetValue("PlayerPools", out Dictionary<string, MatchMakingTicket> cachedData))
            {
                return cachedData;
            }

            // If not in the cache, return null or fetch it from the data source
            return null;
        }
    }
}
