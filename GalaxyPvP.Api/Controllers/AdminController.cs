using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using System.Security.Claims;
using GalaxyPvP.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using GalaxyPvP.Data.Dto.User;
using Azure.Core;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly IPlayerRepository _dbPlayer;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public AdminController(IPlayerRepository dbPlayer, IMapper mapper, GalaxyPvPContext context, IUserRepository userRepository)
        {
            _dbPlayer = dbPlayer;
            _mapper = mapper;
            _context = context;
            _userRepo = userRepository;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            bool isAdmin = await _userRepo.IsAdminByEmail(model.Email);

            if (isAdmin)
            {
                ApiResponse<LoginResponseDTO> loginResponse = await _userRepo.Login(model);
                return ReturnFormatedResponse(loginResponse);
            }
            else
            {
                return Unauthorized("Only Admin can access");
            }

        }

        [HttpGet("GetAllPlayer")]
        public async Task<IActionResult> GetAllPlayer([FromQuery] PageRequest request)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isAdmin = await _userRepo.IsAdminByUserId(userId);

            if (isAdmin)
            {
                ApiResponse<PageResponse<PlayerDto>> response = await _dbPlayer.GetAllPlayer(request);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return BadRequest("Only Admin can access");
            }
        }

        [HttpGet("GetPlayerByNickname")]
        public async Task<IActionResult> GetPlayerByNickname([FromQuery] PageRequest request, string nickname)
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isAdmin = await _userRepo.IsAdminByUserId(userId);

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (isAdmin)
            {
                ApiResponse<PageResponse<PlayerDto>> response = await _dbPlayer.GetByPlayerNickname(request, nickname);
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
            bool isAdmin = await _userRepo.IsAdminByUserId(userId);

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (isAdmin)
            {
                ApiResponse<PlayerUpdateDto> response = await _dbPlayer.Update(updateDto);
                return ReturnFormatedResponse(response);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }
    }
}
