using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GalaxyPvP.Data
{
    public class PlayerRespository : GenericRepository<Player, GalaxyPvPContext>, IPlayerRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public PlayerRespository(GalaxyPvPContext db,IMapper mapper):base(db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PlayerDto>> Get(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "UserId Null");
                }
                var player = await FindAsync(p => p.UserId == userId);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Not Found!");
                }
                PlayerDto reponse = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(401, ex.Message);

            }
        }

        public async Task<ApiResponse<PlayerDto>> Create(PlayerCreateDto playerCreateDto)
        {
            try
            {
                if (playerCreateDto == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Create data is null");
                }
                if (await FindAsync(p => p.Nickname == playerCreateDto.Nickname || p.PlayfabId == playerCreateDto.PlayfabId) != null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Player exists");
                }
                Player player = _mapper.Map<Player>(playerCreateDto);
                player.CreatedAt = DateTime.Now;
                player.UpdatedAt = DateTime.Now;
                Add(player);
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerDto>> Update(PlayerDto playerUpdateDto)
        {
            try
            {
                if (playerUpdateDto == null)
                {
                    return ApiResponse<PlayerDto>.ReturnFailed(401, "Update data is null");
                }

                //Player player = await FindAsync(p => p.Id == playerUpdateDto.Id);
                Player player = await Context.Set<Player>().FirstOrDefaultAsync(p => p.Id == playerUpdateDto.Id);
                if (player == null)
                {
                    return ApiResponse<PlayerDto>.Return404("Player not found");
                }

                player = _mapper.Map<Player>(playerUpdateDto);
                await Context.SaveChangesAsync();
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);
                return ApiResponse<PlayerDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerDto>> Delete(int playerId)
        {
            try
            {
                Player removePlayer = await FindAsync(x => x.Id == playerId);
                PlayerDto responsePlayer = _mapper.Map<PlayerDto>(removePlayer);
                Delete(removePlayer);
                return ApiResponse<PlayerDto>.ReturnResultWith200(responsePlayer);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerDto>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
