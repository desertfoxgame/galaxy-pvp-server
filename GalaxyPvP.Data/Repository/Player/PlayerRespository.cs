using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data
{
    public class PlayerRespository : GenericRepository<Player>, IPlayerRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public PlayerRespository(GalaxyPvPContext db,IMapper mapper):base(db)
        {
            _db = db;
            _mapper = mapper;
        }

    }
}
