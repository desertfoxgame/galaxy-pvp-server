using AutoMapper;
using Azure;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : BaseController
    {
        private readonly IPlayerRepository _dbPlayer;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public PlayerController(IPlayerRepository dbPlayer, IMapper mapper, GalaxyPvPContext context)
        {
            _dbPlayer = dbPlayer;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetPlayerByUserId")]
        public async Task<IActionResult> GetPlayerByUserId(string userId)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.GetByUserId(userId);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetPlayerByPlayerId")]
        public async Task<IActionResult> GetPlayerByPlayerId(string playerId)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.GetByPlayerId(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpPost("CreatePlayer")]
        //[Authorize]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerCreateDto createDto)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.Create(createDto);
            return ReturnFormatedResponse(response);
        }

        [HttpPut("UpdatePlayer")]
        public async Task<IActionResult> UpdatePlayer([FromBody] PlayerUpdateDto updateDto)
        {
            ApiResponse<PlayerUpdateDto> response = await _dbPlayer.Update(updateDto);
            return ReturnFormatedResponse(response);
        }

        [HttpDelete("DeletePlayer")]
        public async Task<IActionResult> DeletePlayer([FromBody] string playerId)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.Delete(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetLeaderboard")]
        public async Task<IActionResult> GetLeaderboard(int amount)
        {
            ApiResponse<List<PlayerDto>> response = await _dbPlayer.GetLeaderboard(amount);
            return ReturnFormatedResponse(response);
        }
    }
}
