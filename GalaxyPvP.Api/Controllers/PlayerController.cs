using AutoMapper;
using Azure;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Http;
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
        public async Task<ApiResponse<Player>> GetPlayer(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<Player>.ReturnFailed(401, "UserId Null");
                }
                var player = await _dbPlayer.GetAsync(p => p.UserId == userId);
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

        [HttpPost("CreatePlayer")]
        public async Task<ApiResponse<PlayerDto>> CreatePlayer([FromBody] PlayerCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Create data is null");
                }
                if (await _dbPlayer.GetAsync(p => p.UserId == createDto.UserId || p.Nickname == p.Nickname || p.PlayfabId == p.PlayfabId) != null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Player exists");
                }
                Player player = _mapper.Map<Player>(createDto);
                player.CreateAt = DateTime.Now.ToLocalTime();
                player.UpdateAt = DateTime.Now.ToLocalTime();
                await _dbPlayer.CreateAsync(player);
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(401, ex.Message);
            }
        }

        [HttpPut("UpdatePlayer")]
        public async Task<ApiResponse<PlayerDto>> UpdatePlayer([FromBody] PlayerUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Update data is null");
                }

                Player player = await _dbPlayer.GetAsync(p => p.Id == updateDto.Id);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.Return404("Player not found");
                }

                player = _mapper.Map<Player>(updateDto);
                player.UpdateAt = DateTime.Now.ToLocalTime();
                await _dbPlayer.SaveAsync();
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(401, ex.Message);
            }
        }

        [HttpDelete("DeletePlayer")]
        public async Task<ApiResponse<string>> DeletePlayer([FromBody] int playerId)
        {
            try
            {
                Player removePlayer = await _dbPlayer.GetAsync(x => x.Id == playerId);
                await _dbPlayer.RemoveAsync(removePlayer);
                return ApiResponse<string>.ReturnSuccess();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
