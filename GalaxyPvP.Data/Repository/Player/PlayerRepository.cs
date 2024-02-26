using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GalaxyPvP.Data
{
    public class PlayerRepository : GenericRepository<Player, GalaxyPvPContext>, IPlayerRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public PlayerRepository(GalaxyPvPContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PlayerDto>> GetByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "UserId Null");
                }
                var player = await FindAsync(p => p.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Not Found!");
                }
                PlayerDto reponse = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);

            }
        }

        public async Task<ApiResponse<PlayerDto>> GetByPlayerId(string playerId)
        {
            try
            {
                if (string.IsNullOrEmpty(playerId))
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "PlayerId Null");
                }
                var player = await FindAsync(p => p.Id == playerId);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Not Found!");
                }
                PlayerDto reponse = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);

            }
        }

        public async Task<ApiResponse<PlayerDto>> Create(string userId, PlayerCreateDto playerCreateDto)
        {
            try
            {
                if (playerCreateDto == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Create data is null");
                }
                else if (await FindAsync(p => p.Nickname == playerCreateDto.Nickname || p.Id == playerCreateDto.Id) != null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Player exists");
                }
                else if (await Context.Set<Player>().FirstOrDefaultAsync(x => x.UserId == userId) != null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "This user has already created player");
                }

                Player player = _mapper.Map<Player>(playerCreateDto);
                if (string.IsNullOrEmpty(player.Id))
                {
                    player.Id = GenerateExtension.GenerateID(16);
                }
                player.CreatedAt = DateTime.Now;
                player.UpdatedAt = DateTime.Now;
                Add(player);
                player.UserId = userId;
                player.Tutorial = 1;
                Context.SaveChanges();
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerUpdateDto>> Update(PlayerUpdateDto playerUpdateDto)
        {
            try
            {
                if (playerUpdateDto == null)
                {
                    return ApiResponse<PlayerUpdateDto>.ReturnFailed(404, "Update data is null");
                }

                //Player player = await FindAsync(p => p.Id == playerUpdateDto.Id);
                Player player = await Context.Set<Player>().FirstOrDefaultAsync(p => p.Nickname == playerUpdateDto.Nickname);
                if (player == null)
                {
                    return ApiResponse<PlayerUpdateDto>.Return404("Player not found");
                }

                _mapper.Map(playerUpdateDto, player);
                player.CreatedAt = DateTime.Now;
                player.UpdatedAt = DateTime.Now;
                await Context.SaveChangesAsync();
                PlayerUpdateDto playerDTO = _mapper.Map<PlayerUpdateDto>(player);
                return ApiResponse<PlayerUpdateDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerUpdateDto>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerDto>> Delete(string userId)
        {
            try
            {
                Player removePlayer = await FindAsync(x => x.UserId == userId);
                PlayerDto responsePlayer = _mapper.Map<PlayerDto>(removePlayer);
                Delete(removePlayer);
                Context.SaveChanges();
                return ApiResponse<PlayerDto>.ReturnResultWith200(responsePlayer);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<List<PlayerDto>>> GetLeaderboard(int amount)
        {
            try
            {
                List<PlayerDto> res = new List<PlayerDto>();
                if (amount > Context.Set<Player>().ToList().Count)
                {
                    amount = Context.Set<Player>().ToList().Count;
                }
                List<Player> list = await Context.Set<Player>().OrderByDescending(x => x.Trophy).Take(amount).ToListAsync();

                res = _mapper.Map<List<PlayerDto>>(list);

                return ApiResponse<List<PlayerDto>>.ReturnResultWith200(res);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PlayerDto>>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<int>> GetPlayerRank(string userId)
        {
            try
            {
                Player player = Context.Set<Player>().FirstOrDefault(x => x.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<int>.ReturnFailed(404, "Player not exist!");
                }

                List<Player> list = await Context.Set<Player>().OrderByDescending(x => x.Trophy).ToListAsync();
                int playerRank = list.FindIndex(p => p.Id == player.Id) + 1;

                return ApiResponse<int>.ReturnResultWith200(playerRank);
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ReturnFailed(404, ex.Message);
            }
        }



        public async Task<ApiResponse<string>> GetPlayerEquipData(string playerId)
        {
            try
            {
                var player = Context.Set<Player>().FirstOrDefault(x => x.Id == playerId);
                if (player == null)
                {
                    return ApiResponse<string>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    return ApiResponse<string>.ReturnResultWith200(player.EquipData);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<UpdateNameDto>> UpdateNickName(string userId, string nickname)
        {
            try
            {
                var player = Context.Set<Player>().FirstOrDefault(x => x.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<UpdateNameDto>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    player.Nickname = nickname;
                    await Context.SaveChangesAsync();
                    return ApiResponse<UpdateNameDto>.ReturnResultWith200(new UpdateNameDto());
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<UpdateNameDto>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerDto>> UpdatePlayerEquipData(string userId, string equipdata)
        {
            try
            {
                var player = Context.Set<Player>().FirstOrDefault(x => x.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    player.EquipData = equipdata;
                    await Context.SaveChangesAsync();

                    return ApiResponse<PlayerDto>.ReturnResultWith200(_mapper.Map<PlayerDto>(player));
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);
            }
        }
        public async Task<ApiResponse<PlayerDto>> UpdatePlayerTutorial(string userId)
        {
            try
            {
                var player = Context.Set<Player>().FirstOrDefault(x => x.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    player.Tutorial = 1;
                    await Context.SaveChangesAsync();

                    return ApiResponse<PlayerDto>.ReturnResultWith200(_mapper.Map<PlayerDto>(player));
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerDto>> UpdatePlayerTrophyByUserId(string userId, int trophy)
        {
            try
            {
                var player = Context.Set<Player>().FirstOrDefault(x => x.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(404, "Player not exist!");
                }
                else
                {
                    player.Trophy += trophy;
                    await Context.SaveChangesAsync();

                    return ApiResponse<PlayerDto>.ReturnResultWith200(_mapper.Map<PlayerDto>(player));
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PageResponse<PlayerDto>>> GetAllPlayer(PageRequest request)
        {
            try
            {
                var players = await Context.Set<Player>().ToListAsync();
                List<PlayerDto> listPlayerDTO = new List<PlayerDto>();
                foreach (var player in players)
                {
                    listPlayerDTO.Add(_mapper.Map<PlayerDto>(player));
                }
                var pageList = PagedList<PlayerDto>.ToPagedList(listPlayerDTO, request.PageNumber, request.PageSize);
                PageResponse<PlayerDto> response = new PageResponse<PlayerDto>
                {
                    TotalCount = pageList.TotalCount,
                    PageSize = pageList.PageSize,
                    CurrentPage = pageList.CurrentPage,
                    TotalPages = pageList.TotalPages,
                    HasNext = pageList.HasNext,
                    HasPrev = pageList.HasPrev,
                    Data = pageList
                };

                return ApiResponse<PageResponse<PlayerDto>>.ReturnResultWith200(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<PageResponse<PlayerDto>>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PageResponse<PlayerDto>>> GetByPlayerNickname(PageRequest request, string nickname)
        {
            try
            {
                var players = await Context.Set<Player>().Where(x => x.Nickname.ToLower().Contains(nickname.ToLower().Trim()) 
                                                                    //|| x.ema.ToLower().Contains(nickname.ToLower().Trim())
                                                                    ).ToListAsync();
                List<PlayerDto> listPlayerDTO = new List<PlayerDto>();
                foreach (var player in players)
                {
                    listPlayerDTO.Add(_mapper.Map<PlayerDto>(player));
                }
                var pageList = PagedList<PlayerDto>.ToPagedList(listPlayerDTO, request.PageNumber, request.PageSize);
                PageResponse<PlayerDto> response = new PageResponse<PlayerDto>
                {
                    TotalCount = pageList.TotalCount,
                    PageSize = pageList.PageSize,
                    CurrentPage = pageList.CurrentPage,
                    TotalPages = pageList.TotalPages,
                    HasNext = pageList.HasNext,
                    HasPrev = pageList.HasPrev,
                    Data = pageList
                };

                return ApiResponse<PageResponse<PlayerDto>>.ReturnResultWith200(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<PageResponse<PlayerDto>>.ReturnFailed(404, ex.Message);
            }
        }

        
    }
}
