using AutoMapper;
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

        public async Task<ApiResponse<Friend>> GetFriendList(int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<Friend>> GetFriendNotification(int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> CreateFriendRequest(FriendRequestDto request)
        {
            try
            {
                if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player1) == null)
                {
                    return ApiResponse<string>.ReturnResultWith200("Player1 not exist!");
                }
                else if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == request.Player2) == null)
                {
                    return ApiResponse<string>.ReturnResultWith200("Player2 not exist!");
                }
                else
                {
                    Friend newFriendRequest = new Friend();
                    newFriendRequest.Player1 = request.Player1;
                    newFriendRequest.Player2 = request.Player2;
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
    }
}
