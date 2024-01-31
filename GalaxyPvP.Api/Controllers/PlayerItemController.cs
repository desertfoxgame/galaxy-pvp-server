using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerItemController : BaseController
    {
        private readonly IPlayerItemRepository _repository;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public PlayerItemController(IPlayerItemRepository repository, IMapper mapper, GalaxyPvPContext context)
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
        public async Task<IActionResult> CreatePlayerItems(ListCreatePlayerItemDto createDto)
        {
            ApiResponse<ListCreatePlayerItemDto> response = await _repository.CreateList(createDto);
            return ReturnFormatedResponse(response);
        }

        [HttpPut("UpdatePlayerItem")]
        //[Authorize]
        public async Task<IActionResult> UpdatePlayerItem(string playerId, PlayerItemUpdateDto updateDto)
        {
            ApiResponse<PlayerItemDto> response = await _repository.Update(playerId, updateDto);
            return ReturnFormatedResponse(response);
        }

        [HttpPut("UpdatePlayerItems")]
        //[Authorize]
        public async Task<IActionResult> UpdatePlayerItems(ListUpdatePlayerItemDto updateDto)
        {
            ApiResponse<ListUpdatePlayerItemDto> response = await _repository.UpdateList(updateDto);
            return ReturnFormatedResponse(response);
        }

        [HttpDelete("DeletePlayerItem")]
        //[Authorize]
        public async Task<IActionResult> DeletePlayerItem(string playerId, int dataId)
        {
            ApiResponse<PlayerItemDto> response = await _repository.Delete(playerId, dataId);
            return ReturnFormatedResponse(response);
        }
    }
}
