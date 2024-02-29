using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Model;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNet.SignalR;
using GalaxyPvP.Data.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using GalaxyPvP.Data.Repository.User;
using Quantum;
using Azure.Core;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : BaseController
    {
        private readonly IFriendRepository _repoFriend;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private GalaxyPvPContext _context;

        public FriendController(IFriendRepository repoFriend, IMapper mapper, GalaxyPvPContext context, IUserRepository userRepository)
        {
            _repoFriend = repoFriend;
            _mapper = mapper;
            _context = context;
            _userRepo = userRepository;
        }

        [HttpGet("GetFriendList")]
        [Authorize]
        public async Task<IActionResult> GetFriendList(string playerId)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<List<PlayerDto>> response = await _repoFriend.GetFriendList(playerId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpGet("GetInviteList")]
        [Authorize]
        public async Task<IActionResult> GetInviteList(string playerId)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<List<PlayerDto>> response = await _repoFriend.GetInviteList(playerId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPost("SentFriendRequest")]
        [Authorize]
        public async Task<IActionResult> SentFriendRequest([FromBody] FriendRequestDto request)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<string> response = await _repoFriend.CreateFriendRequest(request);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPut("UpdateFriendRequest")]
        [Authorize]
        public async Task<IActionResult> UpdateFriendRequest(short state, FriendRequestDto request)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<string> response = await _repoFriend.UpdateFriendRequest(state, request);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpGet("GetFriendNotification")]
        [Authorize]
        public async Task<IActionResult> GetFriendNotification(string playerId)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<int> response = await _repoFriend.GetFriendNotification(playerId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpPost("DeleteFriendRequest")]
        [Authorize]
        public async Task<IActionResult> DeleteFriendRequest([FromBody] FriendRequestDto request)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<string> response = await _repoFriend.DeleteFriendRequest(request);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }
    }
}
