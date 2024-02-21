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
        private readonly IAdminRepository _adminRepo;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public AdminController(IPlayerRepository dbPlayer, IMapper mapper, GalaxyPvPContext context, IUserRepository userRepository, IAdminRepository adminRepo)
        {
            _dbPlayer = dbPlayer;
            _mapper = mapper;
            _context = context;
            _userRepo = userRepository;
            _adminRepo = adminRepo;
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllPlayer([FromQuery] PageRequest request)
        {
            ApiResponse<PageResponse<AdminPlayerDTO>> response = await _adminRepo.GetAllPlayer(request);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("SearchPlayer")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> SearchPlayer([FromQuery] PageRequest request, string input)
        {
            ApiResponse<PageResponse<AdminPlayerDTO>> response = null;
            if (input.Trim() != "/")
            {
                response = await _adminRepo.SearchPlayer(request, input);

            }
            else
            {
                response = await _adminRepo.GetAllPlayer(request);

            }
            return ReturnFormatedResponse(response);
        }

        [HttpPut("UpdatePlayer")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePlayer([FromBody] PlayerUpdateDto updateDto)
        {
            ApiResponse<PlayerUpdateDto> response = await _dbPlayer.Update(updateDto);
            return ReturnFormatedResponse(response);
        }
        
        [HttpPut("UpdateUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO updateDto)
        {
            ApiResponse<UserDTO> response = await _userRepo.Update(updateDto);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetPlayer")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPlayer(string playerId)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.GetByPlayerId(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUser(string playerId)
        {
            ApiResponse<UserDTO> response = await _userRepo.GetByPlayerId(playerId);
            return ReturnFormatedResponse(response);
        }
    }
}
