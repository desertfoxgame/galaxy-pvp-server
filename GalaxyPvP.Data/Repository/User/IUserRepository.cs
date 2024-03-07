using GalaxyPvP.Data.Dto;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data.Repository.User
{
    public interface IUserRepository: IGenericRepository<GalaxyUser>
    {
        bool IsUniqueUser(string username);
        Task<ApiResponse<LoginResponseDTO>> Login(LoginRequestDTO loginRequestDTO);
        Task<ApiResponse<LoginResponseDTO>> LoginWithWallet(LoginRequestDTO request);
        Task<ApiResponse<UserDTO>> RegisterWithEmail(RegisterRequestDTO registerationRequestDTO);
        Task<ApiResponse<UserDTO>> RegisterWithWallet(RegisterRequestDTO registerationRequestDTO);
        Task<ApiResponse<string>> ForgotPassword(string email);
        Task<ApiResponse<string>> ResetPassword(string verifyCode, string newPassword);

        Task<ApiResponse<UserDTO>> GetById(string userId);
        Task<ApiResponse<UserDTO>> AuthorizeUser(string userId, string token);
        Task<ApiResponse<UserDTO>> GetByEmail(string email);
        Task<ApiResponse<UserDTO>> GetByWallet(string wallet);
        Task<ApiResponse<UserDTO>> GetByUserName(string userName);
        Task<ApiResponse<UserDTO>> GetByPlayerId(string playerId);
        Task<ApiResponse<UserDTO>> Update(UserDTO playerUpdateDto);
        Task<bool> IsAdminByEmail(string email);
        Task<bool> IsAdminByUserId(string userId);
        Task<ApiResponse<string>> EmailConfirm(string verifycode);
        Task<ApiResponse<string>> UpdateWalletToUser(UpdateUserWalletDTO request);
        Task<ApiResponse<string>> UpdateEmailToUser(UpdateUserWalletDTO request);
    }
}
