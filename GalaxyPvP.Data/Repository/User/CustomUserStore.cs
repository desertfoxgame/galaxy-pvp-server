using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Repository.User
{
    public class CustomUserStore : IUserStore<GalaxyUser>
    {
        public Task<IdentityResult> CreateAsync(GalaxyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(GalaxyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<GalaxyUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<GalaxyUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedUserNameAsync(GalaxyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(GalaxyUser user, CancellationToken cancellationToken)
        {
            string userId = GenerateCustomUserId();
            return Task.FromResult(userId);
        }

        public Task<string?> GetUserNameAsync(GalaxyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(GalaxyUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(GalaxyUser user, string? userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(GalaxyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private string GenerateCustomUserId()
        {
            Guid guid = Guid.NewGuid();
            string userId = guid.ToString("N").Substring(0, 42); // Get the first 42 characters
            return userId;
        }
    }
}
