using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Extensions;
using UserManagement.Common.GenericRespository;

namespace GalaxyPvP.Data.Repository.User
{
    public interface IUserRepository: IGenericRepository<GalaxyUser>
    {
        bool IsUniqueUser(string username);
        Task<ApiResponse<LoginResponseDTO>> Login(LoginRequestDTO loginRequestDTO);
        Task<ApiResponse<UserDTO>> Register(RegisterRequestDTO registerationRequestDTO);
    }
}
