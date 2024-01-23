using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Extensions;
using GalaxyPvP.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        private Dictionary<string, MatchMakingTicket> PlayerPools = new Dictionary<string, MatchMakingTicket>();

        public MatchMakingController(UserManager<GalaxyUser> userManager, IPlayerRepository playerManager, IMapper mapper, 
            UserInfoToken userInfoToken,
            IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _playerRepository = playerManager;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _httpContext = httpContext;
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
            if(PlayerPools.ContainsKey(player.Id))
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "Already in queued"));
            }
            var ticket = _mapper.Map<MatchMakingTicket>(createDto);
            ticket.PlayerId = player.Id;
            ticket.SubmitedTime = DateTime.UtcNow;
            PlayerPools.Add(player.Id, ticket);
            return ReturnFormatedResponse(ApiResponse<string>.ReturnSuccess());
        }
    }
}
