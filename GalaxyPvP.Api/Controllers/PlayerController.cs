using AutoMapper;
using GalaxyPvP.Data;
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
        public PlayerController(IPlayerRepository dbPlayer, IMapper mapper)
        {
            _dbPlayer = dbPlayer;
            _mapper = mapper;
        }

        //[HttpGet("{userId}", Name = "api/get-player")]
        //public async Task<ActionResult<ApiResponse>> GetPlayer(string userId)
        //{
        //    try
        //    {
        //        if(string.IsNullOrEmpty(userId))
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(_response);
        //        }
        //        var player = await _dbPlayer.GetAsync(p => p.UserId == userId);
        //        if(player == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(_response);
        //        }
        //        _response.Result = _mapper.Map<PlayerDto>(player);
        //        _response.StatusCode = HttpStatusCode.OK;
        //        return Ok(_response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages
        //             = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}

        //[HttpGet("{userId}", Name = "api/get-player")]
        //public async Task<ActionResult> GetPlayer(string userId)
        //{
        //    try
        //    {
        //        ApiResponse<Player> response = _dbPlayer.GetAsync(x => x.UserId == userId);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            response.StatusCode = int.Parse(HttpStatusCode.BadRequest.ToString());
        //            return BadRequest(HttpStatusCode.BadRequest);
        //        }
        //        var player = await _dbPlayer.GetAsync(p => p.UserId == userId);
        //        if (player == null)
        //        {
        //            //_response.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(HttpStatusCode.NotFound);
        //        }
        //        //_response.Result = _mapper.Map<PlayerDto>(player);
        //        //_response.StatusCode = HttpStatusCode.OK;
        //        return Ok(player);
        //    }
        //    catch (Exception ex)
        //    {
        //        //_response.IsSuccess = false;
        //        //_response.ErrorMessages
        //        //     = new List<string>() { ex.ToString() };
        //    }
        //    //return _response;
        //    return BadRequest();
        //}

        //[HttpPost]
        //public async Task<ActionResult<ApiResponse>> CreatePlayer([FromBody] PlayerCreateDto createDto)
        //{
        //    try
        //    {
        //        if(createDto == null)
        //        {
        //            return BadRequest(createDto);
        //        }
        //        if(await _dbPlayer.GetAsync(p=>p.UserId == createDto.UserId) != null)
        //        {
        //            ModelState.AddModelError("ErrorMessages", "Player exists");
        //            return BadRequest(ModelState);
        //        }
        //        Player player = _mapper.Map<Player>(createDto);
        //        player.CreateAt = DateTime.Now.ToLocalTime();
        //        player.UpdateAt = DateTime.Now.ToLocalTime();
        //        await _dbPlayer.CreateAsync(player);
        //        _response.Result = _mapper.Map<PlayerDto>(player);
        //        _response.StatusCode = HttpStatusCode.OK;
        //        return Ok(_response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages
        //             = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}
    }
}
