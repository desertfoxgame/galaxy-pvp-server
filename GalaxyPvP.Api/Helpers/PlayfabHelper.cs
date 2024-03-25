using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlayFab;
using PlayFab.AdminModels;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using UserDataRecord = PlayFab.ServerModels.UserDataRecord;
using GetPlayerCombinedInfoRequest = PlayFab.ServerModels.GetPlayerCombinedInfoRequest;

namespace GalaxyPvP.Api.Helpers
{
    public class PlayfabHelper
    {
        public static async Task<LoginPlayfabResponse> LoginWithWallet(string wallet)
        {
            LoginWithServerCustomIdRequest request = new()
            {
                ServerCustomId = wallet
            };
            var loginPlayfab = await PlayFabServerAPI.LoginWithServerCustomIdAsync(request);
            if (loginPlayfab.Error != null)
                return LoginPlayfabResponse.ReturnError(loginPlayfab.Error.ErrorMessage);
            else
                return LoginPlayfabResponse.ReturnSuccess(loginPlayfab.Result.PlayFabId);
        }

        public static async Task<LoginPlayfabResponse> LoginWithEmail(string email, string password)
        {
            LoginWithEmailAddressRequest request = new()
            {
                Email = email,
                Password = password,
            };
            var loginResp = await PlayFabClientAPI.LoginWithEmailAddressAsync(request);
            if (loginResp.Error != null)
                return LoginPlayfabResponse.ReturnError(loginResp.Error.ErrorMessage);
            else
                return LoginPlayfabResponse.ReturnSuccess(loginResp.Result.PlayFabId);
        }

        public static async Task<PlayfabDataResponse> GetPlayfabData(string playfabId)
        {
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
                        ShowContactEmailAddresses = true,
                    }
                }
            };
            var combinedInfoResult = await PlayFabServerAPI.GetPlayerCombinedInfoAsync(combinedInfoRequest);
            if (combinedInfoResult.Error != null)
            {
                return PlayfabDataResponse.ReturnError(combinedInfoResult.Error.ErrorMessage);
            }

            var profile = combinedInfoResult.Result.InfoResultPayload.PlayerProfile;
            var inventory = combinedInfoResult.Result.InfoResultPayload.UserInventory;
            var userData = combinedInfoResult.Result.InfoResultPayload?.UserData;
            var readonlyData = combinedInfoResult.Result.InfoResultPayload?.UserReadOnlyData;
            var statistic = combinedInfoResult.Result.InfoResultPayload?.PlayerStatistics;

            string email = profile.ContactEmailAddresses?.Count > 0 ? profile.ContactEmailAddresses[0].EmailAddress : string.Empty;
            string nickname = profile.DisplayName ?? string.Empty;
            string walletaddress = readonlyData.TryGetValue("publicaddress", out UserDataRecord? wallet) ? wallet.Value : string.Empty;
            string verification = readonlyData.TryGetValue("verification", out UserDataRecord? vefiry) ? vefiry.Value : "Pending";
            string confirmTermsOfService = readonlyData.TryGetValue("ConfirmTermsOfService", out UserDataRecord? confirm) ? vefiry.Value : "false";

            string currentWinStreaks = userData.TryGetValue("CurrentWinStreaks", out UserDataRecord? CurrentWinStreaks) ? CurrentWinStreaks.Value : "0";
            string mvp = userData.TryGetValue("MVP", out UserDataRecord? MVP) ? MVP.Value : "0";
            string totalGames = userData.TryGetValue("TotalGames", out UserDataRecord? TotalGames) ? TotalGames.Value : "0";
            string winGames = userData.TryGetValue("WinGames", out UserDataRecord? WinGames) ? WinGames.Value : "0";
            string winStreaks = userData.TryGetValue("WinStreaks", out UserDataRecord? WinStreaks) ? WinStreaks.Value : "0";
            string tutorial = userData.TryGetValue("tutorial", out UserDataRecord? Tutorial) ? Tutorial.Value : "0";
            string developer = userData.TryGetValue("developer", out UserDataRecord? Developer) ? Developer.Value : "0";
            string isAdmin = userData.TryGetValue("isAdmin", out UserDataRecord? IsAdmin) ? IsAdmin.Value : "0";
            string equip = userData.TryGetValue("NftInformation", out UserDataRecord? Infomation) ? Infomation.Value : string.Empty;

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
            MigrateUserRequestDTO migrationRequestDTO = new()
            {
                PlayfabID = playfabId,
                Email = email,
                Nickname = nickname,
                WalletAddress = walletaddress,
                Win = int.Parse(winGames),
                TotalGames = int.Parse(totalGames),
                MVP = int.Parse(mvp),
                WinStreak = int.Parse(winStreaks),
                WinStreakCurrent = int.Parse(currentWinStreaks),
                PlayerItems = playerItems,
                EquipData = equip,
                Trophy = trophy,
                Tutorial = short.Parse(tutorial),
                isAdmin = short.Parse(isAdmin),
                Developer = short.Parse(developer),
                Verification = verification,
                ConfirmTermsOfService = confirmTermsOfService,
            };
            return PlayfabDataResponse.ReturnSuccess(migrationRequestDTO);
        }
    }

    public class LoginPlayfabResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string PlayfabId { get; set; }

        public LoginPlayfabResponse(bool success, string errorMessage, string playfabId)
        {
            Success = success;
            ErrorMessage = errorMessage;
            PlayfabId = playfabId;
        }
        public static LoginPlayfabResponse ReturnSuccess(string playfabId)
        {
            return new LoginPlayfabResponse(true, string.Empty, playfabId);
        }

        public static LoginPlayfabResponse ReturnError(string errorMessage)
        {
            return new LoginPlayfabResponse(false, errorMessage, string.Empty);
        }

    }

    public class PlayfabDataResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public MigrateUserRequestDTO? Data { get; set; }
        public PlayfabDataResponse(bool success, string error, MigrateUserRequestDTO? data)
        {
            Success = success;
            ErrorMessage = error;
            Data = data;
        }

        public static PlayfabDataResponse ReturnSuccess(MigrateUserRequestDTO data)
        {
            return new PlayfabDataResponse(true, string.Empty, data);
        }

        public static PlayfabDataResponse ReturnError(string errorMessage)
        {
            return new PlayfabDataResponse(false, errorMessage, null);
        }
    }
}
