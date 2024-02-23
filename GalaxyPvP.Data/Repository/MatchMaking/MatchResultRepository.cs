using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.MatchSubmit;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Repository.MatchMaking
{
    public class MatchResultRepository : GenericRepository<MatchResult, GalaxyPvPContext>, IMatchResultRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public MatchResultRepository(GalaxyPvPContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ApiResponse<MatchResultDto>> Create(MatchResultDto matchResultDto)
        {
            try
            {
                MatchResult matchResult = new MatchResult();
                matchResult.MatchId = matchResultDto.MatchId;

                string matchDataJson = JsonConvert.SerializeObject(matchResultDto.MatchData);
                matchResult.MatchData = matchDataJson;
                matchResult.CreatedAt = DateTime.Now;
                matchResult.UpdatedAt = DateTime.Now;
                Add(matchResult);
                await Context.SaveChangesAsync();

                return ApiResponse<MatchResultDto>.ReturnResultWith200(matchResultDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<MatchResultDto>.ReturnFailed(404, ex.Message);

            }
        }

        public Task<ApiResponse<MatchResult>> Delete(string matchId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<MatchResultDto>> Get(string matchId)
        {
            try
            {
                MatchResultDto res = new MatchResultDto();
                MatchResult matchResult = await Context.Set<MatchResult>().FirstOrDefaultAsync(x => x.MatchId == matchId);
                if (matchResult == null)
                {
                    return ApiResponse<MatchResultDto>.ReturnFailed(404, "Match Result not exist!");
                }

                MatchDataDto matchDataDto = JsonConvert.DeserializeObject<MatchDataDto>(matchResult.MatchData);
                res.MatchId = matchId;
                res.MatchData = matchDataDto;
                return ApiResponse<MatchResultDto>.ReturnResultWith200(res);
            }
            catch (Exception ex)
            {
                return ApiResponse<MatchResultDto>.ReturnFailed(404, ex.Message);
            }

        }

        public Task<ApiResponse<PlayerRegisterMatchDto>> Update(PlayerRegisterMatchDto matchUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}
