using GalaxyPvP.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GalaxyPvP.Data.Context
{
    public class GalaxyPvPContext : IdentityDbContext<GalaxyUser>
    {
        public GalaxyPvPContext(DbContextOptions<GalaxyPvPContext> options) : base(options) { }

        public DbSet<GalaxyUser> GalaxyUsers { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<PlayerItem> PlayerItem { get; set; }
        public DbSet<GameConfig> GameConfigs { get; set; }
        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GameConfig>().HasData(
                new GameConfig() { Key = "Version_Android", Value = "3.0.0" },
                new GameConfig() { Key = "Version_iOS", Value = "3.0.0" },
                new GameConfig() { Key = "Version_Windows", Value = "3.0.0" }
                );

            builder.Entity<Friend>()
                .HasOne(f => f.Player1)
                .WithMany(p => p.FriendsAsPlayer1)
                .HasForeignKey(f => f.Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friend>()
                .HasOne(f => f.Player2)
                .WithMany(p => p.FriendsAsPlayer2)
                .HasForeignKey(f => f.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
