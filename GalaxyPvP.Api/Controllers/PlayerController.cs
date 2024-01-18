using AutoMapper;
using Azure;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
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
        public async Task<ActionResult> GetPlayer(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest();
                }
                var player = await _dbPlayer.GetAsync(p => p.UserId == userId);
                if (player == null)
                {
                    return NotFound();
                }
                return Ok(player);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }

        [HttpPost("CreatePlayer")]
        public async Task<ActionResult> CreatePlayer([FromBody] PlayerCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest(createDto);
                }
                if (await _dbPlayer.GetAsync(p => p.UserId == createDto.UserId || p.Nickname == p.Nickname || p.PlayfabId == p.PlayfabId) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Player exists");
                    return BadRequest(ModelState);
                }
                Player player = _mapper.Map<Player>(createDto);
                player.CreateAt = DateTime.Now.ToLocalTime();
                player.UpdateAt = DateTime.Now.ToLocalTime();
                await _dbPlayer.CreateAsync(player);
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return Ok(playerDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("UpdatePlayer")]
        public async Task<ActionResult> UpdatePlayer([FromBody] PlayerUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return BadRequest(updateDto);
                }

                Player player = await _dbPlayer.GetAsync(p => p.Id == updateDto.Id);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                player = _mapper.Map<Player>(updateDto);
                player.UpdateAt = DateTime.Now.ToLocalTime();
                await _dbPlayer.SaveAsync();
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return Ok(playerDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("DeletePlayer")]
        public async Task<ActionResult> DeletePlayer([FromBody] int playerId)
        {
            try
            {
                Player removePlayer = await _dbPlayer.GetAsync(x => x.Id == playerId);
                await _dbPlayer.RemoveAsync(removePlayer);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
