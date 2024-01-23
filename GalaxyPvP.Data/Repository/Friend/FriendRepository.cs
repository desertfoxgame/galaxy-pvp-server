using AutoMapper;
using Azure.Core;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
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
                    return ApiResponse<List<PlayerDto>>.ReturnFailed(401, "Player not exist!");
                }
                else
                {
                    List<Friend> listFriend = Context.Set<Friend>()
                                                    .Include(f => f.Player1)
                                                    .Include(f => f.Player2)
                                                    .Where(f => (f.Player1Id == playerId || f.Player2Id == playerId) && f.state == 1)
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
                return ApiResponse<List<PlayerDto>>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> CreateFriendRequest(FriendRequestDto request)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player1) == null)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Player1 not exist!");
                }
                else if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player2) == null)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Player2 not exist!");
                }
                else if (await Context.Set<Friend>().FirstOrDefaultAsync(x => (x.Player1Id == request.Player1 && x.Player2Id == request.Player2) ||
                                                                        (x.Player1Id == request.Player2 && x.Player2Id == request.Player1)) != null)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Request has been sent!");
                }
                else if (request.Player1 == request.Player2)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Player1 and Player2 can't be the same Id!");
                }
                else
                {
                    Friend newFriendRequest = new Friend();
                    newFriendRequest.Player1Id = request.Player1;
                    newFriendRequest.Player2Id = request.Player2;
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
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> UpdateFriendRequest(short state, FriendRequestDto request)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player1) == null)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Player1 not exist!");
                }
                else if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player2) == null)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Player2 not exist!");
                }
                else if (request.Player1 == request.Player2)
                {
                    return ApiResponse<string>.ReturnFailed(401, "Player1 and Player2 can't be the same Id!");
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
                        return ApiResponse<string>.ReturnFailed(401, "Friend Request not exist!");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<int>> GetFriendNotification(string playerId)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId) == null)
                {
                    return ApiResponse<int>.ReturnFailed(401, "Player not exist!");
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
                return ApiResponse<int>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
