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
        Task<ApiResponse<UserDTO>> Register(RegisterRequestDTO registerationRequestDTO);

        //Task<ApiResponse<UserDTO>> GetById(string userId);
        //Task<ApiResponse<UserDTO>> GetByEmail(string email);
        //Task<ApiResponse<UserDTO>> GetByUserName(string userName);
        //Task<ApiResponse<UserDTO>> Create(UserDTO userCreateDto);
        //Task<ApiResponse<UserDTO>> Update(PlayerItemDto playerUpdateDto);
        //Task<ApiResponse<UserDTO>> Delete(int playerId);
    }
}
