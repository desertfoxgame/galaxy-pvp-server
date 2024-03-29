﻿using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data
{
    public interface IPlayerRepository : IGenericRepository<Player>
    {
        Task<ApiResponse<PlayerDto>> GetByUserId(string userId);
        Task<ApiResponse<PageResponse<PlayerDto>>> GetAllPlayer(PageRequest request);
        Task<ApiResponse<string>> GetPlayerEquipData(string userId);
        Task<ApiResponse<PlayerDto>> UpdatePlayerEquipData(string userId, string equipdata);
        Task<ApiResponse<PlayerDto>> GetByPlayerId(string playerId);
        Task<ApiResponse<PageResponse<PlayerDto>>> GetByPlayerNickname(PageRequest request, string nickname);
        Task<ApiResponse<PlayerDto>> Create(string userId, PlayerCreateDto playerCreateDto);
        Task<ApiResponse<PlayerUpdateDto>> Update(PlayerUpdateDto playerUpdateDto);
        Task<ApiResponse<PlayerDto>> Delete(string userId);
        Task<ApiResponse<int>> GetPlayerRank(string? userId);
        Task<ApiResponse<List<PlayerDto>>> GetLeaderboard(int amount);
        Task<ApiResponse<PlayerDto>> UpdatePlayerTrophyByUserId(string userId, int trophy);
        Task<ApiResponse<string>> UpdatePlayerTutorial(string userId);
        Task<ApiResponse<string>> ConfirmTermsOfService(string userId);
        Task<ApiResponse<DanielResponse>> DanielGetPlayer(string playerId);
        Task<ApiResponse<UpdateNameDto>> UpdateNickName(string userId, string nickname);
        Task<ApiResponse<PlayerDto>> MigrateDataCreate(string userId, Player player);

    }
}