﻿using AutoMapper;
using Azure.Core;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.AspNet.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class FriendRepository : GenericRepository<Friend, GalaxyPvPContext>, IFriendRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepo;

        public FriendRepository(GalaxyPvPContext db, IMapper mapper, IPlayerRepository playerRepo) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _playerRepo = playerRepo;
        }

        public async Task<ApiResponse<List<PlayerDto>>> GetFriendList(string playerId)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId) == null)
                {
                    return ApiResponse<List<PlayerDto>>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    // stage == 1 ---- friend list
                    // stage == 2 ---- invite list
                    List<Friend> listFriend = Context.Set<Friend>()
                                                    .Include(f => f.Player1)
                                                    .Include(f => f.Player2)
                                                    .Where(f => (f.Player1Id == playerId || f.Player2Id == playerId) && f.state == 1 && f.IsDeleted == false)
                                                    .ToList();
                    List<PlayerDto> list = new List<PlayerDto>();
                    foreach (var f in listFriend)
                    {
                        string friendId = (f.Player1Id == playerId) ? f.Player2Id : f.Player1Id;
                        Player player = Context.Set<Player>().FirstOrDefault(f => f.Id == friendId);
                        PlayerDto playerDto = _mapper.Map<PlayerDto>(player);
                        if(player != null)
                        {
                            list.Add(playerDto);
                        }
                    }

                    return ApiResponse<List<PlayerDto>>.ReturnResultWith200(list);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PlayerDto>>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> CreateFriendRequest(FriendRequestDto request)
        {
            try
            {
                Player player2 = await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player2 || x.Nickname == request.Player2);
                if ( player2 == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player does not exist!");
                }
                string player2Id = player2.Id;
                Friend friend = await Context.Set<Friend>().FirstOrDefaultAsync(x => (x.Player1Id == request.Player1 && x.Player2Id == player2Id) ||
                                                                        (x.Player1Id == player2Id && x.Player2Id == request.Player1));
                if (friend != null)
                {
                    if (friend.IsDeleted)
                    {
                        return ApiResponse<string>.ReturnFailed(404, "Player does not exist!");
                    } else if (friend.state == 0)
                    {
                        return ApiResponse<string>.ReturnFailed(404, "Request has been sent!");
                    }
                }
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player1) == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player does not exist!");
                }
                else if (request.Player1 == player2Id)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Invalid friend account id");
                }
                else
                {
                    Friend newFriendRequest = new Friend();
                    newFriendRequest.Player1Id = request.Player1;
                    newFriendRequest.Player2Id = player2Id;
                    newFriendRequest.state = 0;
                    newFriendRequest.CreatedAt = DateTime.Now;
                    newFriendRequest.UpdatedAt = DateTime.Now;
                    Add(newFriendRequest);
                    Context.SaveChanges();
                    return ApiResponse<string>.ReturnResultWith200("Sent!");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> UpdateFriendRequest(short state, FriendRequestDto request)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player1) == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player1 not exist!");
                }
                else if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player2) == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player2 not exist!");
                }
                else if (request.Player1 == request.Player2)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player1 and Player2 can't be the same Id!");
                }
                else
                {
                    Friend friendRequest = await Context.Set<Friend>().FirstOrDefaultAsync(x => (x.Player1Id == request.Player1 && x.Player2Id == request.Player2) ||
                                                                                                (x.Player1Id == request.Player2 && x.Player2Id == request.Player1));
                    if (friendRequest != null)
                    {
                        friendRequest.state = state;
                        friendRequest.UpdatedAt = DateTime.Now;
                        Context.SaveChanges();
                        return ApiResponse<string>.ReturnResultWith200("Successful!");
                    }
                    else
                    {
                        return ApiResponse<string>.ReturnFailed(404, "Friend Request not exist!");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<int>> GetFriendNotification(string playerId)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId) == null)
                {
                    return ApiResponse<int>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    List<Friend> listFriend = Context.Set<Friend>()
                                                    .Include(f => f.Player1)
                                                    .Include(f => f.Player2)
                                                    .Where(f => (f.Player1Id == playerId || f.Player2Id == playerId) && f.state == 0)
                                                    .ToList();

                    return ApiResponse<int>.ReturnResultWith200(listFriend.Count);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> DeleteFriendRequest(FriendRequestDto request)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player1) == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player1 not exist!");
                }
                else if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player2) == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player2 not exist!");
                }
                else if (request.Player1 == request.Player2)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player1 and Player2 can't be the same Id!");
                }
                else
                {
                    Friend friendRequest = await Context.Set<Friend>().FirstOrDefaultAsync(x => (x.Player1Id == request.Player1 && x.Player2Id == request.Player2) ||
                                                                                                (x.Player1Id == request.Player2 && x.Player2Id == request.Player1));
                    if (friendRequest != null)
                    {
                        friendRequest.IsDeleted = true;
                        friendRequest.UpdatedAt = DateTime.Now;
                        Context.SaveChanges();
                        return ApiResponse<string>.ReturnResultWith200("Successful!");
                    }
                    else
                    {
                        return ApiResponse<string>.ReturnFailed(404, "Friend Request not exist!");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<List<PlayerDto>>> GetInviteList(string playerId)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId) == null)
                {
                    return ApiResponse<List<PlayerDto>>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    // stage == 1 ---- friend list
                    // stage == 2 ---- invite list
                    List<Friend> listFriend = Context.Set<Friend>()
                                                    .Include(f => f.Player1)
                                                    .Include(f => f.Player2)
                                                    .Where(f => (f.Player2Id == playerId) && f.state == 0 && f.IsDeleted == false)
                                                    .ToList();
                    List<PlayerDto> list = new List<PlayerDto>();
                    foreach (var f in listFriend)
                    {
                        string friendId = f.Player1Id;
                        Player player = Context.Set<Player>().FirstOrDefault(f => f.Id == friendId);
                        PlayerDto playerDto = _mapper.Map<PlayerDto>(player);
                        if (player != null)
                        {
                            list.Add(playerDto);
                        }
                    }

                    return ApiResponse<List<PlayerDto>>.ReturnResultWith200(list);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PlayerDto>>.ReturnFailed(404, ex.Message);
            }
        }
    }
}
