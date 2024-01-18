using Microsoft.EntityFrameworkCore;
using GalaxyPvP.Data.Model;

namespace GalaxyPvP.Data
{
    public interface IPlayerRepository: IGenericRepository<Player>
    {
        
    }
}