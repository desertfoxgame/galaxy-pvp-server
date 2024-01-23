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

        public async Task<ApiResponse<List<Player>>> GetFriendList(string playerId)
        {
            throw new NotImplementedException();

            //try
            //{
            //    // Check if the player with the given ID exists
            //    Player player = await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId);
            //    if (player == null)
            //    {
            //        // Player not found, return a failure response
            //        return null; // or ApiResponse<List<Player>>.ReturnFailed(...) if you prefer
            //    }

            //    // Retrieve the list of friends for the given player
            //    List<Friend> listFriend = Context.Set<Friend>()
            //        .Include(f => f.Player1)
            //        .Include(f => f.Player2)
            //        .Where(f => (f.Player1Id == playerId || f.Player2Id == playerId) && f.state == 1)
            //        .ToList();

            //    // Create a list to store the friend players
            //    List<Player> friendList = new List<Player>();

            //    // Iterate through each friend and add the corresponding player to the list
            //    foreach (var friend in listFriend)
            //    {
            //        // Determine the friend's ID (the other player's ID in the friendship)
            //        string friendId = (friend.Player1Id == playerId) ? friend.Player2Id : friend.Player1Id;

            //        // Retrieve the friend player and add it to the list
            //        Player friendPlayer = await Context.Set<Player>().FirstOrDefaultAsync(p => p.Id == friendId);
            //        if (friendPlayer != null)
            //        {
            //            friendList.Add(friendPlayer);
            //        }
            //    }

            //    return friendList;
            //}
            //catch (Exception ex)
            //{
            //    // Handle exceptions if needed
            //    return null; // or ApiResponse<List<Player>>.ReturnFailed(...) if you prefer
            //}
            //try
            //{
            //    if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId) == null)
            //    {
            //        return ApiResponse<List<Player>>.ReturnFailed(401, "Player not exist!");
            //    }
            //    else
            //    {
            //        List<Friend> listFriend = Context.Set<Friend>()
            //                                        .Include(f => f.Player1)
            //                                        .Include(f => f.Player2)
            //                                        .Where(f => (f.Player1Id == playerId || f.Player2Id == playerId) && f.state == 1)
            //                                        .ToList();
            //        List<Player> list = new List<Player>();
            //        foreach (var f in listFriend)
            //        {
            //            string friendId = (f.Player1Id == playerId) ? f.Player2Id : f.Player1Id;
            //            Player player = Context.Set<Player>().FirstOrDefault(f => f.Id == friendId);
            //            player.FriendsAsPlayer1 = Context.Set<Player>().FirstOrDefault(f => f.Id == playerId);
            //            list.Add(player);
            //        }

            //        return ApiResponse<List<Player>>.ReturnResultWith200(list);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return ApiResponse<List<Player>>.ReturnFailed(401, ex.Message);
            //}
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

        public async Task<ApiResponse<Friend>> UpdateFriendRequest(int itemId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<Friend>> GetFriendNotification(int itemId)
        {
            throw new NotImplementedException();
        }
    }
}
