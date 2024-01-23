using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : BaseController
    {
        private readonly IFriendRepository _repoFriend;
        private readonly IMapper _mapper;

        public FriendController(IFriendRepository repoFriend, IMapper mapper)
        {
            _repoFriend = repoFriend;
            _mapper = mapper;
        }

        [HttpGet("GetFriendList")]
        public async Task<IActionResult> GetFriendList(string playerId)
        {
            //ApiResponse<List<Player>> response = await _repoFriend.GetFriendList(playerId);
            //return ReturnFormatedResponse(response);
            ApiResponse<List<Player>> response = await _repoFriend.GetFriendList(playerId);

            // Use JsonSerializerOptions with ReferenceHandler.Preserve
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                // Add any other serialization options if needed
            };

            // Serialize the object using JsonSerializer with options
            var json = JsonSerializer.Serialize(response, options);

            return Ok(json);
        }

        [HttpPost("SentFriendRequest")]
        public async Task<IActionResult> SentFriendRequest([FromBody] FriendRequestDto request)
        {
            ApiResponse<string> response = await _repoFriend.CreateFriendRequest(request);
            return ReturnFormatedResponse(response);
        }
    }
}
