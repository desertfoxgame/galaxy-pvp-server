using GalaxyPvP.Data.Dto.MigrationDB;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data.Repository.User
{
    public interface IMigrationDataRepository : IGenericRepository<GalaxyUser>
    {
        Task<ApiResponse<MigrateUserResponseDTO>> MigrationUser(MigrateUserRequestDTO request);
        Task<ApiResponse<MigrateUserResponseDTO>> MigrationUser(MigrateUserRequestDTO request, string password);
        Task<ApiResponse<string>> AddItemData();
        Task<ApiResponse<string>> DeleteMigrationUser(string userId);
    }
}
