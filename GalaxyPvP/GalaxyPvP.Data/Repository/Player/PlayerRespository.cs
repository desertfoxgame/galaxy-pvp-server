using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data
{
    public class PlayerRespository : Repository<Player, GalaxyPvPContext>,
        IPlayerRepository
    {
        private GalaxyPvPContext _dbContext;
        protected PlayerRespository(GalaxyPvPContext context) : base(context)
        {
            _dbContext = context;
        }

    }
}
