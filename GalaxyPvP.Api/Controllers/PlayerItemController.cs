using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerItemController : BaseController
    {
        private readonly IPlayerItemRepository _repository;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public PlayerItemController(IPlayerItemRepository repository, IMapper mapper, GalaxyPvPContext context, IUserRepository userRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
            _userRepo = userRepository;
        }

        [HttpGet("GetPlayerItem")]
        [Authorize]
        public async Task<IActionResult> GetPlayerItem(int itemId)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerItemDto> response = await _repository.Get(itemId, userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }

        }

        [HttpGet("GetAllPlayerItem")]
        [Authorize]
        public async Task<IActionResult> GetPlayerItem()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<ListPlayerItemDto> response = await _repository.GetAll(userId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpPost("CreatePlayerItem")]
        [Authorize]
        public async Task<IActionResult> CreatePlayerItem(PlayerItemCreateDto createDto)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerItemDto> response = await _repository.Create(userId, createDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpPost("CreatePlayerItems")]
        [Authorize]
        public async Task<IActionResult> CreatePlayerItems(ListCreatePlayerItemDto createDto)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<ListCreatePlayerItemDto> response = await _repository.CreateList(userId, createDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        [HttpPut("UpdatePlayerItem")]
        [Authorize]
        public async Task<IActionResult> UpdatePlayerItem(PlayerItemUpdateDto updateDto)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerItemDto> response = await _repository.Update(userId, updateDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpPut("UpdatePlayerItems")]
        [Authorize]
        public async Task<IActionResult> UpdatePlayerItems(ListUpdatePlayerItemDto updateDto)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<ListUpdatePlayerItemDto> response = await _repository.UpdateList(userId, updateDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }

        [HttpDelete("DeletePlayerItem")]
        [Authorize]
        public async Task<IActionResult> DeletePlayerItem(string playerId, int dataId)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                ApiResponse<PlayerItemDto> response = await _repository.Delete(userId, dataId);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
            
        }
    }
}
