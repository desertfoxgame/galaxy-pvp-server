﻿using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Model;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNet.SignalR;

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
        [Authorize]
        public async Task<IActionResult> GetFriendList(string playerId)
        {
            ApiResponse<List<PlayerDto>> response = await _repoFriend.GetFriendList(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetInviteList")]
        [Authorize]
        public async Task<IActionResult> GetInviteList(string playerId)
        {
            ApiResponse<List<PlayerDto>> response = await _repoFriend.GetInviteList(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpPost("SentFriendRequest")]
        [Authorize]
        public async Task<IActionResult> SentFriendRequest([FromBody] FriendRequestDto request)
        {
            ApiResponse<string> response = await _repoFriend.CreateFriendRequest(request);
            return ReturnFormatedResponse(response);
        }

        [HttpPut("UpdateFriendRequest")]
        [Authorize]
        public async Task<IActionResult> UpdateFriendRequest(short state, FriendRequestDto request)
        {
            ApiResponse<string> response = await _repoFriend.UpdateFriendRequest(state, request);
            return ReturnFormatedResponse(response);
        }

        [HttpGet("GetFriendNotification")]
        [Authorize]
        public async Task<IActionResult> GetFriendNotification(string playerId)
        {
            ApiResponse<int> response = await _repoFriend.GetFriendNotification(playerId);
            return ReturnFormatedResponse(response);
        }

        [HttpPost("DeleteFriendRequest")]
        [Authorize]
        public async Task<IActionResult> DeleteFriendRequest([FromBody] FriendRequestDto request)
        {
            ApiResponse<string> response = await _repoFriend.DeleteFriendRequest(request);
            return ReturnFormatedResponse(response);
        }
    }
}
