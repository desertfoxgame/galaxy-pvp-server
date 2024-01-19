using AutoMapper;
using Azure;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
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
        public async Task<ApiResponse<PlayerDto>> GetPlayer(string userId)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.Get(userId);
            return response;
        }

        [HttpPost("CreatePlayer")]
        public async Task<ApiResponse<PlayerDto>> CreatePlayer([FromBody] PlayerCreateDto createDto)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.Create(createDto);
            return response;
        }

        [HttpPut("UpdatePlayer")]
        public async Task<ApiResponse<PlayerDto>> UpdatePlayer([FromBody] PlayerDto updateDto)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.Update(updateDto);
            return response;
        }

        [HttpDelete("DeletePlayer")]
        public async Task<ApiResponse<PlayerDto>> DeletePlayer([FromBody] int playerId)
        {
            ApiResponse<PlayerDto> response = await _dbPlayer.Delete(playerId);
            return response;
        }

        [HttpGet("GetPlayerItem")]
        [Authorize]
        public async Task<ApiResponse<Player>> GetPlayerItem(int playerId)
        {
            try
            {
                //if (string.IsNullOrEmpty(playerId))
                //{
                //    return ApiResponse<Player>.ReturnFailed(401, "UserId Null");
                //}
                var player = _dbPlayer.FindBy(p => p.Id == playerId).FirstOrDefault();
                if (player == null)
                {
                    return ApiResponse<Player>.ReturnFailed(401, "Not Found!");
                }
                return ApiResponse<Player>.ReturnResultWith200(player);
            }
            catch (Exception ex)
            {
                return ApiResponse<Player>.ReturnFailed(401, ex.Message);

            }
        }
    }
}
