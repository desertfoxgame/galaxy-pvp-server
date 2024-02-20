using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Data.Repository.MatchMaking;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Dto.MatchSubmit;
using Microsoft.AspNetCore.Authorization;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchResultController : BaseController
    {
        private readonly IMatchResultRepository _repository;
        private readonly IMapper _mapper;
        private GalaxyPvPContext _context;

        public MatchResultController(IMatchResultRepository repository, IMapper mapper, GalaxyPvPContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetMatchResult")]
        [Authorize]
        public async Task<IActionResult> GetMatchResult(string matchId)
        {
            ApiResponse<MatchResultDto> response = await _repository.Get(matchId);
            return ReturnFormatedResponse(response);
        }

        [HttpPost("CreateMatchResult")]
        [Authorize]
        public async Task<IActionResult> CreateMatchResult(MatchResultDto matchResultDto)
        {
            ApiResponse<MatchResultDto> response = await _repository.Create(matchResultDto);
            return ReturnFormatedResponse(response);
        }
    }
}
