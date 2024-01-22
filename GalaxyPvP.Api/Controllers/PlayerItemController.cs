using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Data.Repository.Player;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerItemController : BaseController
    {
        private readonly IPlayerItemRespository _repository;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public PlayerItemController(IPlayerItemRespository repository, IMapper mapper, GalaxyPvPContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetPlayerItem")]
        //[Authorize]
        public async Task<IActionResult> GetPlayerItem(int itemId)
        {
            ApiResponse<PlayerItemDto> response = await _repository.Get(itemId);
            return ReturnFormatedResponse(response);
        }
        
        [HttpGet("GetAllPlayerItem")]
        //[Authorize]
        public async Task<IActionResult> GetPlayerItem(string playerId)
        {
            ApiResponse<ListPlayerItemDto> response = await _repository.GetAll(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpPost("CreatePlayerItem")]
        //[Authorize]
        public async Task<IActionResult> CreatePlayerItem(PlayerItemCreateDto createDto)
        {
            ApiResponse<PlayerItemDto> response = await _repository.Create(createDto);
            return ReturnFormatedResponse(response);
        }
        
        [HttpPost("CreatePlayerItems")]
        //[Authorize]
        public async Task<IActionResult> CreatePlayerItems(ListPlayerItemDto createDto)
        {
            ApiResponse<ListPlayerItemDto> response = await _repository.CreateList(createDto);
            return ReturnFormatedResponse(response);
        }

        [HttpPut("UpdatePlayerItem")]
        //[Authorize]
        public async Task<IActionResult> UpdatePlayerItem(PlayerItemDto updateDto)
        {
            ApiResponse<PlayerItemDto> response = await _repository.Update(updateDto);
            return ReturnFormatedResponse(response);
        }

        [HttpDelete("DeletePlayerItem")]
        //[Authorize]
        public async Task<IActionResult> DeletePlayerItem(int itemId)
        {
            ApiResponse<PlayerItemDto> response = await _repository.Delete(itemId);
            return ReturnFormatedResponse(response);
        }
    }
}
