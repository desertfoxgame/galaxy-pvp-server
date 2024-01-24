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

        public UserController(ILogger<UserController> logger, IUserRepository userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
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
                    InfoRequestParameters = new()
                    {
                        GetPlayerProfile = true,
                        GetUserData = true,
                        GetUserInventory = true,
                        GetUserReadOnlyData = true,
                        ProfileConstraints = new()
                        {
                            ShowDisplayName = true,
                        },
                        UserDataKeys =
                        [
                            "CurrentWinStreaks",
                            "MVP",
                            "TotalGames",
                            "WinGames",
                            "WinStreaks",
                            "tutorial",
                            "developer"
                        ],
                        UserReadOnlyDataKeys = 
                        [
                            "publicaddress"
                        ]
                    }
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

                string currentWinStreaks = userData.TryGetValue("CurrentWinStreaks", out UserDataRecord? CurrentWinStreaks) ? CurrentWinStreaks.Value : string.Empty;
                string mvp = userData.TryGetValue("MVP", out UserDataRecord? MVP) ? MVP.Value : string.Empty;
                string totalGames = userData.TryGetValue("TotalGames", out UserDataRecord? TotalGames) ? TotalGames.Value : string.Empty;
                string winGames = userData.TryGetValue("WinGames", out UserDataRecord? WinGames) ? WinGames.Value : string.Empty;
                string winStreaks = userData.TryGetValue("WinStreaks", out UserDataRecord? WinStreaks) ? WinStreaks.Value : string.Empty;
                string tutorial = userData.TryGetValue("tutorial", out UserDataRecord? Tutorial) ? Tutorial.Value : string.Empty;
                string developer = userData.TryGetValue("developer", out UserDataRecord? Developer) ? Developer.Value : string.Empty;

                // Migrate data and return response here
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
