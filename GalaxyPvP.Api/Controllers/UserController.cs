using Azure.Core;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.MigrationDB;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Mvc;
using PlayFab;
using PlayFab.ClientModels;
using Serilog;
using System.Net;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepo;
        private readonly IMigrationDataRepository _migrationDataRepo;
        
        public UserController(ILogger<UserController> logger, IUserRepository userRepo, IMigrationDataRepository migrationDataRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
            _migrationDataRepo = migrationDataRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            ApiResponse<LoginResponseDTO> loginResponse = await _userRepo.Login(model);
            if (loginResponse.StatusCode == 400)
            {
                LoginWithEmailAddressRequest playfabRequest = new()
                {
                    Email = model.Email,
                    Password = model.Password,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true,
                        GetUserData = true,
                        GetUserInventory = true,
                        GetUserReadOnlyData = true,
                        ProfileConstraints = new PlayerProfileViewConstraints
                        {
                            ShowDisplayName = true,
                        },
                    },
                };
                var playfabResp = await PlayFabClientAPI.LoginWithEmailAddressAsync(playfabRequest);
                if (playfabResp.Error != null)
                {
                    loginResponse = ApiResponse<LoginResponseDTO>.ReturnFailed(404, playfabResp.Error.ErrorMessage);
                    return ReturnFormatedResponse(loginResponse);
                }

                string playfabId = playfabResp.Result.PlayFabId;
                string email = model.Email;
                string password = model.Password;

                var profile = playfabResp.Result.InfoResultPayload.PlayerProfile;
                var inventory = playfabResp.Result.InfoResultPayload.UserInventory;
                var userData = playfabResp.Result.InfoResultPayload?.UserData;
                var readonlyData = playfabResp.Result.InfoResultPayload?.UserReadOnlyData;

                string nickname = profile.DisplayName == null ? string.Empty : profile.DisplayName;
                string walletaddress = readonlyData.TryGetValue("publicaddress", out UserDataRecord? wallet) ? wallet.Value : string.Empty;

                string currentWinStreaks = userData.TryGetValue("CurrentWinStreaks", out UserDataRecord? CurrentWinStreaks) ? CurrentWinStreaks.Value : "0";
                string mvp = userData.TryGetValue("MVP", out UserDataRecord? MVP) ? MVP.Value : "0";
                string totalGames = userData.TryGetValue("TotalGames", out UserDataRecord? TotalGames) ? TotalGames.Value : "0";
                string winGames = userData.TryGetValue("WinGames", out UserDataRecord? WinGames) ? WinGames.Value : "0";
                string winStreaks = userData.TryGetValue("WinStreaks", out UserDataRecord? WinStreaks) ? WinStreaks.Value : "0";
                string tutorial = userData.TryGetValue("tutorial", out UserDataRecord? Tutorial) ? Tutorial.Value : string.Empty;
                string developer = userData.TryGetValue("developer", out UserDataRecord? Developer) ? Developer.Value : string.Empty;

                string[] playerItems = new string[5000];
                for (int i = 0; i < inventory?.Count; i++)
                {
                    playerItems[i] = (inventory[i].DisplayName);
                }

                // Migrate data and return response here
                MigrateUserRequestDTO migrationRequestDTO = new MigrateUserRequestDTO();
                migrationRequestDTO.PlayfabID = playfabId;
                migrationRequestDTO.Email = email;
                migrationRequestDTO.Nickname = nickname;
                migrationRequestDTO.WalletAddress = walletaddress;
                migrationRequestDTO.WinGames = int.Parse(winGames);
                migrationRequestDTO.TotalGames = int.Parse(totalGames);
                migrationRequestDTO.MVP = int.Parse(mvp);
                migrationRequestDTO.WinStreaks = int.Parse(winStreaks);
                migrationRequestDTO.CurrentWinStreak = int.Parse(currentWinStreaks);
                migrationRequestDTO.PlayerItems = playerItems;

                ApiResponse<MigrateUserResponseDTO> response = await _migrationDataRepo.MigrationUser(migrationRequestDTO);
                if(response.Success)
                {
                    return ReturnFormatedResponse(response);
                }
                else
                {
                    return ReturnFormatedResponse(ApiResponse<MigrateUserResponseDTO>.ReturnFailed(401, response.Errors));
                }
            }
            return ReturnFormatedResponse(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            var user = await _userRepo.Register(model);
            return ReturnFormatedResponse(user);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string userId)
        {
            var user = await _userRepo.GetById(userId);
            return ReturnFormatedResponse(user);
        }

        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail(string userEmail)
        {
            var user = await _userRepo.GetByEmail(userEmail);
            return ReturnFormatedResponse(user);
        }

        [HttpGet("GetByUserName")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var user = await _userRepo.GetByUserName(userName);
            return ReturnFormatedResponse(user);
        }
    }
}
