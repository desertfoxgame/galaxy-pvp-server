using GalaxyPvP.Data.Dto.MatchSubmit;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Repository.MatchMaking
{
    public interface IMatchResultRepository : IGenericRepository<MatchResult>
    {
        Task<ApiResponse<MatchResultDto>> Get(string matchId);
        Task<ApiResponse<MatchResultDto>> Create(MatchResultDto matchResultDto);
        Task<ApiResponse<PlayerRegisterMatchDto>> Update(PlayerRegisterMatchDto matchUpdateDto);
        Task<ApiResponse<MatchResult>> Delete(string matchId);

    }
}
