using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data.Repository.User
{
    public interface IMigrationDataRepository : IGenericRepository<GalaxyUser>
    {
        Task<ApiResponse<string>> MigrationUser(MigrateUserRequestDTO request);

    }
}
