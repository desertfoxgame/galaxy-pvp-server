using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public interface IFriendRepository
    {
        Task<ApiResponse<string>> CreateFriendRequest(FriendRequestDto request);
        Task<ApiResponse<List<Player>>> GetFriendList(string playerId);
        Task<ApiResponse<Friend>> GetFriendNotification(int itemId);
        Task<ApiResponse<Friend>> UpdateFriendRequest(int itemId);
    }
}
