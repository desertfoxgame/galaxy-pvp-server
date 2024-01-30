using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Data.Dto.Leaderboard;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardRepository _repository;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public LeaderboardController(ILeaderboardRepository repository, IMapper mapper, GalaxyPvPContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetTopList")]
        //[Authorize]
        public async Task<IActionResult> GetTopList(int amount)
        {
            ApiResponse<ListLeaderboardDTO> response = await _repository.GetTopList(amount);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("UpdateList")]
        //[Authorize]
        public async Task<IActionResult> UpdateList()
        {
            ApiResponse<ListLeaderboardDTO> response = await _repository.Update();
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetByRank")]
        //[Authorize]
        public async Task<IActionResult> GetByRank(int rank)
        {
            ApiResponse<LeaderboardDTO> response = await _repository.GetByRank(rank);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetByPlayerId")]
        //[Authorize]
        public async Task<IActionResult> GetByPlayerId(string playerId)
        {
            ApiResponse<LeaderboardDTO> response = await _repository.GetByPlayerId(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpPost("AddAllPlayer")]
        //[Authorize]
        public async Task<IActionResult> AddAllPlayer()
        {
            ApiResponse<string> response = await _repository.AddAllPlayer();
            return ReturnFormatedResponse(response);
        }
    }
}
