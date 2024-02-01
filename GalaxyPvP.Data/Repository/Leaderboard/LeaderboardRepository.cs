using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Leaderboard;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GalaxyPvP.Data
{
    public class LeaderboardRepository: GenericRepository<Leaderboard, GalaxyPvPContext>, ILeaderboardRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public LeaderboardRepository(GalaxyPvPContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ListLeaderboardDTO>> GetTopList(int amount)
        {
            List<Leaderboard> topList = SortList(amount);
            ListLeaderboardDTO response = new ListLeaderboardDTO();
            response.list = new List<LeaderboardDTO>();

            foreach (Leaderboard leaderboard in topList)
            {
                LeaderboardDTO dto = _mapper.Map<LeaderboardDTO>(leaderboard);
                dto.Trophy = leaderboard.Player.Trophy;
                response.list.Add(dto);
            }

            if (topList.Count > 0 )
            {
                return ApiResponse<ListLeaderboardDTO>.ReturnResultWith200(response);
            }
            else
            {
                return ApiResponse<ListLeaderboardDTO>.ReturnFailed(401, "List not found!");
            }
        }

        public async Task<ApiResponse<LeaderboardDTO>> GetByRank(int rank)
        {
            try
            {
                Leaderboard leaderboard = Context.Set<Leaderboard>().Include(x => x.Player).FirstOrDefault(x => x.Rank == rank);
                if(leaderboard == null)
                {
                    return ApiResponse<LeaderboardDTO>.ReturnFailed(401, "Player at this rank not exist!");
                }
                else
                {
                    LeaderboardDTO leaderboardDTO = _mapper.Map<LeaderboardDTO>(leaderboard);
                    leaderboardDTO.Trophy = leaderboard.Player.Trophy;
                    return ApiResponse<LeaderboardDTO>.ReturnResultWith200(leaderboardDTO);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<LeaderboardDTO>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<LeaderboardDTO>> GetByPlayerId(string playerId)
        {
            try
            {
                Leaderboard leaderboard = Context.Set<Leaderboard>().Include(x => x.Player).FirstOrDefault(x => x.PlayerId == playerId);
                if (leaderboard == null)
                {
                    return ApiResponse<LeaderboardDTO>.ReturnFailed(401, "This player not exist!");
                }
                else
                {
                    LeaderboardDTO leaderboardDTO = _mapper.Map<LeaderboardDTO>(leaderboard);
                    leaderboardDTO.Trophy = leaderboard.Player.Trophy;
                    return ApiResponse<LeaderboardDTO>.ReturnResultWith200(leaderboardDTO);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<LeaderboardDTO>.ReturnFailed(401, ex.Message);
            }
        }

        public Task<ApiResponse<LeaderboardDTO>> Create(PlayerDto playerDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> AddAllPlayer()
        {
            try
            {
                List<Player> players = Context.Set<Player>().ToList();
                foreach (Player player in players)
                {
                    Leaderboard leaderboard = Context.Set<Leaderboard>().Where(x => x.PlayerId == player.Id).FirstOrDefault();
                    if(leaderboard == null)
                    {
                        leaderboard = new Leaderboard();
                        leaderboard.PlayerId = player.Id;
                        Context.Set<Leaderboard>().Add(leaderboard);
                    }
                    leaderboard.DisplayName = player.Nickname;
                    leaderboard.Rank = 0;
                }
                Context.SaveChanges();
                return ApiResponse<string>.ReturnResultWith200("Success");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> AddPlayer(string playerId)
        {
            try
            {
                Player? player = Context.Set<Player>().FirstOrDefault(x => x.Id == playerId);
                Leaderboard? leaderboard = Context.Set<Leaderboard>().Where(x => x.PlayerId == player.Id).FirstOrDefault();
                if (leaderboard == null)
                {
                    leaderboard = new Leaderboard();
                    leaderboard.PlayerId = player.Id;
                    Context.Set<Leaderboard>().Add(leaderboard);

                }
                leaderboard.DisplayName = player.Nickname;
                leaderboard.Rank = 0;

                await Context.SaveChangesAsync();
                SortList(0);

                return ApiResponse<string>.ReturnResultWith200("Success");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<ListLeaderboardDTO>> Update()
        {
            List<Leaderboard> topList = SortList(0);
            ListLeaderboardDTO response = new ListLeaderboardDTO();
            response.list = new List<LeaderboardDTO>();

            foreach (Leaderboard leaderboard in topList)
            {
                LeaderboardDTO dto = _mapper.Map<LeaderboardDTO>(leaderboard);
                dto.Trophy = Context.Set<Player>().FirstOrDefault(x => x.Id == leaderboard.PlayerId).Trophy;
                response.list.Add(dto);
            }

            if (topList.Count > 0)
            {
                return ApiResponse<ListLeaderboardDTO>.ReturnResultWith200(response);
            }
            else
            {
                return ApiResponse<ListLeaderboardDTO>.ReturnFailed(401, "List not found!");
            }
        }

        List<Leaderboard> SortList(int amount = 0)
        {
            List<Leaderboard> topList = new List<Leaderboard> ();
            if (amount > 0)
            {
                topList = Context.Set<Leaderboard>().Include(x => x.Player)
                                        .OrderByDescending(x => x.Player.Trophy).Take(amount).ToList();
            }
            else
            {
                topList = Context.Set<Leaderboard>().Include(x => x.Player)
                                        .OrderByDescending(x => x.Player.Trophy).ToList();
            }

            if(topList.Count > 0)
            {
                int count = 1;
                foreach (var top in topList)
                {
                    top.Rank = count;
                    count++;
                }
                Context.SaveChanges();
            }
            return topList;
        }


    }
}
