using Azure.Core;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto;
using GalaxyPvP.Data.Dto.MigrationDB;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using GalaxyPvP.Helper;
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
    public class GalaxyFightClubController : BaseController
    {
        private readonly IUserRepository _userRepo;
        private readonly IMigrationDataRepository _migrationDataRepo;


        public GalaxyFightClubController(IUserRepository userRepo, IMigrationDataRepository migrationDataRepo)
        {
            _userRepo = userRepo;
            _migrationDataRepo = migrationDataRepo;
        }

        //[HttpGet("loginWithWallet")]
        //public async Task<IActionResult> LoginWithWallet([FromBody] LoginRequestDTO request)
        //{
        //    string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    if (adminApiKey == GalaxyExtensions.AdminApiKey)
        //    {
        //        var user = await _userRepo.LoginWithWallet(request);
        //        return ReturnFormatedResponse(user);
        //    }
        //    else
        //        return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        //}

        [HttpPost("registerWithEmail")]
        public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterRequestDTO model)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey) 
            {
                var user = await _userRepo.RegisterWithEmail(model);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }

        [HttpPost("registerWithWallet")]
        public async Task<IActionResult> RegisterWithWallet([FromBody] RegisterRequestDTO model)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey) 
            {
                var user = await _userRepo.RegisterWithWallet(model);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));

        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey) 
            {
                var user = await _userRepo.ForgotPassword(email);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(string verifyCode, string newPassword)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey) 
            {
                var user = await _userRepo.ResetPassword(verifyCode, newPassword);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));

        }

        [HttpPost("verifyEmail")]
        public async Task<IActionResult> Verification(string verifycode)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey) 
            {
                var user = await _userRepo.EmailConfirm(verifycode);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }

        [HttpPost("updateWallet")]
        public async Task<IActionResult> UpdateWallet([FromBody] UpdateUserWalletDTO dto)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey)
            {
                var user = await _userRepo.UpdateWalletToUser(dto);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }

        [HttpPost("updateEmail")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateUserWalletDTO dto)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey)
            {
                var user = await _userRepo.UpdateEmailToUser(dto);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }

        [HttpGet("getUserInfoByEmail")]
        public async Task<IActionResult> GetUserInfoByEmail(string email)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey)
            {
                var user = await _userRepo.GetByEmail(email);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }

        [HttpGet("getUserInfoByWallet")]
        public async Task<IActionResult> GetUserInfoByWallet(string wallet)
        {
            string adminApiKey = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (adminApiKey == GalaxyExtensions.AdminApiKey)
            {
                var user = await _userRepo.GetByWallet(wallet);
                return ReturnFormatedResponse(user);
            }
            else
                return ReturnFormatedResponse(ApiResponse<string>.ReturnFailed(401, "UnAuthorized"));
        }
    }
}
