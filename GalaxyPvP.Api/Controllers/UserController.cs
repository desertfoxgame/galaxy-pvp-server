using Azure.Core;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.MigrationDB;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNetCore.Mvc;
using PlayFab;
using PlayFab.AdminModels;
using PlayFab.ServerModels;
using Serilog;
using System.Net;
using System.Security.Claims;
using UserDataRecord = PlayFab.ServerModels.UserDataRecord;


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

        [HttpPost("GetPath")]
        public async Task<IActionResult> GetPath()
        {
            string htmlTemplatePath = Environment.GetEnvironmentVariable("HOME") + "\\galaxy-pvp-us.scm.azurewebsites.net\\wwwroot\\emailTemplate.json";
            try 
            {
                string htmlContentTemplate = System.IO.File.ReadAllText(htmlTemplatePath);
                return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200(htmlContentTemplate));
            } catch (Exception ex)
            {
                return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200(ex.Message));
            }
            
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            ApiResponse<LoginResponseDTO> loginResponse = await _userRepo.Login(model);
            if (loginResponse.StatusCode == 400)
            {
                LookupUserAccountInfoRequest userInfoRequest = new()
                {
                    Email = model.Email,
                };

                var userInfoResp = await PlayFabAdminAPI.GetUserAccountInfoAsync(userInfoRequest);
                if (userInfoResp.Error != null)
                {
                    loginResponse = ApiResponse<LoginResponseDTO>.ReturnFailed(404, userInfoResp.Error.ErrorMessage);
                    return ReturnFormatedResponse(loginResponse);
                }

                string playfabId = userInfoResp.Result.UserInfo.PlayFabId;

                GetPlayerCombinedInfoRequest combinedInfoRequest = new()
                {
                    PlayFabId = playfabId,
                    InfoRequestParameters = new()
                    {
                        GetPlayerProfile = true,
                        GetUserData = true,
                        GetUserInventory = true,
                        GetUserReadOnlyData = true,
                        GetPlayerStatistics = true,
                        ProfileConstraints = new()
                        {
                            ShowDisplayName = true,
                        }
                    }
                };

                var combinedInfoResult = await PlayFabServerAPI.GetPlayerCombinedInfoAsync(combinedInfoRequest);
                if (combinedInfoResult.Error != null)
                {
                    loginResponse = ApiResponse<LoginResponseDTO>.ReturnFailed(404, combinedInfoResult.Error.ErrorMessage);
                    return ReturnFormatedResponse(loginResponse);
                }

                var profile = combinedInfoResult.Result.InfoResultPayload.PlayerProfile;
                var inventory = combinedInfoResult.Result.InfoResultPayload.UserInventory;
                var userData = combinedInfoResult.Result.InfoResultPayload?.UserData;
                var readonlyData = combinedInfoResult.Result.InfoResultPayload?.UserReadOnlyData;
                var statistic = combinedInfoResult.Result.InfoResultPayload?.PlayerStatistics;

                string email = model.Email;
                string nickname = profile.DisplayName ?? string.Empty;
                string walletaddress = readonlyData.TryGetValue("publicaddress", out UserDataRecord? wallet) ? wallet.Value : string.Empty;

                string currentWinStreaks = userData.TryGetValue("CurrentWinStreaks", out UserDataRecord? CurrentWinStreaks) ? CurrentWinStreaks.Value : "0";
                string mvp = userData.TryGetValue("MVP", out UserDataRecord? MVP) ? MVP.Value : "0";
                string totalGames = userData.TryGetValue("TotalGames", out UserDataRecord? TotalGames) ? TotalGames.Value : "0";
                string winGames = userData.TryGetValue("WinGames", out UserDataRecord? WinGames) ? WinGames.Value : "0";
                string winStreaks = userData.TryGetValue("WinStreaks", out UserDataRecord? WinStreaks) ? WinStreaks.Value : "0";
                string tutorial = userData.TryGetValue("tutorial", out UserDataRecord? Tutorial) ? Tutorial.Value : "0";
                string developer = userData.TryGetValue("developer", out UserDataRecord? Developer) ? Developer.Value : "0";
                string isAdmin = userData.TryGetValue("isAdmin", out UserDataRecord? IsAdmin) ? IsAdmin.Value : "0";

                // current equipment, add it to table
                // we need 1 api to update equip data
                string equip = userData.TryGetValue("NftInformation", out UserDataRecord? Infomation) ? Infomation.Value : string.Empty;

                // add trophy to leaderboard
                int trophy = 0;

                for (int i = 0; i < statistic?.Count; i++)
                {
                    if (statistic[i].StatisticName == "Trophy")
                        trophy = statistic[i].Value;
                }

                string[] playerItems = new string[inventory.Count];
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
                migrationRequestDTO.Win = int.Parse(winGames);
                migrationRequestDTO.TotalGames = int.Parse(totalGames);
                migrationRequestDTO.MVP = int.Parse(mvp);
                migrationRequestDTO.WinStreak = int.Parse(winStreaks);
                migrationRequestDTO.WinStreakCurrent = int.Parse(currentWinStreaks);
                migrationRequestDTO.PlayerItems = playerItems;
                migrationRequestDTO.EquipData = equip;
                migrationRequestDTO.Trophy = trophy;
                migrationRequestDTO.Tutorial = short.Parse(tutorial);
                migrationRequestDTO.isAdmin = short.Parse(isAdmin);
                migrationRequestDTO.Developer = short.Parse(developer);

                ApiResponse<MigrateUserResponseDTO> response = await _migrationDataRepo.MigrationUser(migrationRequestDTO);
                if(response.Success)
                {
                    // send verify code

                    //return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith200("Success!"));
                    return ReturnFormatedResponse(ApiResponse<string>.ReturnResultWith201(""));
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

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userRepo.ForgotPassword(email);
            return ReturnFormatedResponse(user);
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(string verifyCode, string newPassword)
        {
            var user = await _userRepo.ResetPassword(verifyCode, newPassword);
            return ReturnFormatedResponse(user);
        }

        [HttpGet("GetById")]
        [Authorize]
        public async Task<IActionResult> GetById()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ApiResponse<UserDTO> userAuthorize = await _userRepo.AuthorizeUser(userId, jwtToken);
            if (userAuthorize.Success)
            {
                var user = await _userRepo.GetById(userId);
                return ReturnFormatedResponse(user);
            }
            else
            {
                return ReturnFormatedResponse(userAuthorize);
            }
        }

        //[HttpGet("GetByEmail")]
        //[Authorize]
        //public async Task<IActionResult> GetByEmail(string userEmail)
        //{
        //    var user = await _userRepo.GetByEmail(userEmail);
        //    return ReturnFormatedResponse(user);
        //}

        //[HttpGet("GetByUserName")]
        //[Authorize]
        //public async Task<IActionResult> GetByUserName(string userName)
        //{
        //    var user = await _userRepo.GetByUserName(userName);
        //    return ReturnFormatedResponse(user);
        //}
    }
}
