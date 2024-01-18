using GalaxyPvP.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GalaxyPvP.Data.Context
{
    public class GalaxyPvPContext:IdentityDbContext<GalaxyUser>
    {
        public GalaxyPvPContext(DbContextOptions<GalaxyPvPContext> options):base(options) { }

        public DbSet<GalaxyUser> GalaxyUsers { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<PlayerItem> PlayerItem { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
