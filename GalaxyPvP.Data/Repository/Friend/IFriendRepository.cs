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
        Task<ApiResponse<List<PlayerDto>>> GetFriendList(string playerId);
        Task<ApiResponse<int>> GetFriendNotification(string playerId);
        Task<ApiResponse<string>> UpdateFriendRequest(short state, FriendRequestDto friend);
        Task <ApiResponse<string>> DeleteFriendRequest (FriendRequestDto friend);
        Task<ApiResponse<List<PlayerDto>>> GetInviteList(string playerId);
    }
}
