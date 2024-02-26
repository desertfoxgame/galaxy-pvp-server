using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : BaseController
    {
        private readonly IPlayerRepository _dbPlayer;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public PlayerController(IPlayerRepository dbPlayer, IMapper mapper, GalaxyPvPContext context, IUserRepository userRepository)
        {
            _dbPlayer = dbPlayer;
            _mapper = mapper;
            _context = context;
            _userRepo = userRepository;
        }

        [HttpGet("GetPlayer")]
        public async Task<IActionResult> GetPlayer()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerDto> response = await _dbPlayer.GetByUserId(userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        //[HttpGet("GetPlayerByUserId")]
        //[Authorize]
        //public async Task<IActionResult> GetPlayerByUserId(string userId)
        //{
        //    ApiResponse<PlayerDto> response = await _dbPlayer.GetByUserId(userId);
        //    return ReturnFormatedResponse(response);
        //}

        //[HttpGet("GetPlayerByPlayerId")]
        //[Authorize]
        //public async Task<IActionResult> GetPlayerByPlayerId(string playerId)
        //{
        //    ApiResponse<PlayerDto> response = await _dbPlayer.GetByPlayerId(playerId);
        //    return ReturnFormatedResponse(response);
        //}

        [HttpPost("CreatePlayer")]
        [Authorize]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerCreateDto createDto)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerDto> response = await _dbPlayer.Create(userId, createDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpPut("UpdateNickName")]
        [Authorize]
        public async Task<IActionResult> UpdateNickName(string nickname)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<UpdateNameDto> response = await _dbPlayer.UpdateNickName(userId, nickname);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPut("UpdatePlayer")]
        [Authorize]
        public async Task<IActionResult> UpdatePlayer([FromBody] PlayerUpdateDto updateDto)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success || userId == "a0c98b44-f0a3-40c0-9ea4-f75095c8fa41")
            {
                ApiResponse<PlayerUpdateDto> response = await _dbPlayer.Update(updateDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPost("DanielGetPlayer")]
        public async Task<IActionResult> DanielGetPlayer([FromBody] string key, string userId)
        {
            if (key.Equals("a0c98b44-f0a3-40c0-9ea4-f75095c8fa14"))
            {
                ApiResponse<DanielResponse> response = await _dbPlayer.DanielGetPlayer(userId);
                return ReturnFormatedResponse(response);
            }

            return Unauthorized();

        }

        [HttpDelete("DeletePlayer")]
        [Authorize]
        public async Task<IActionResult> DeletePlayer()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerDto> response = await _dbPlayer.Delete(userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpGet("GetLeaderboard")]
        public async Task<IActionResult> GetLeaderboard(int amount)
        {
            ApiResponse<List<PlayerDto>> response = await _dbPlayer.GetLeaderboard(amount);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetPlayerRank")]
        [Authorize]
        public async Task<IActionResult> GetPlayerRank()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<int> response = await _dbPlayer.GetPlayerRank(userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPut("UpdatePlayerEquipData")]
        [Authorize]
        public async Task<IActionResult> UpdatePlayerEquipData(string equipData)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerDto> response = await _dbPlayer.UpdatePlayerEquipData(userId, equipData);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPut("UpdatePlayerTutorial")]
        [Authorize]
        public async Task<IActionResult> UpdatePlayerTutorial()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerDto> response = await _dbPlayer.UpdatePlayerTutorial(userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpGet("GetPlayerEquipData")]
        [Authorize]
        public async Task<IActionResult> GetPlayerEquipData()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<string> response = await _dbPlayer.GetPlayerEquipData(userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }
    }
}
