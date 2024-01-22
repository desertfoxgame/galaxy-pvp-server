using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Model;

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

        [HttpPost("SentFriendRequest")]
        public async Task<IActionResult> SentFriendRequest([FromBody] FriendRequestDto request)
        {
            ApiResponse<string> response = await _repoFriend.CreateFriendRequest(request);
            return ReturnFormatedResponse(response);
        }
    }
}
