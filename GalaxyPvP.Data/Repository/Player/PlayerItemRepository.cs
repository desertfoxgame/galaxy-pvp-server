using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GalaxyPvP.Data.Repository.Player
{
    public class PlayerItemRepository : GenericRepository<PlayerItem, GalaxyPvPContext>, IPlayerItemRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepo;

        public PlayerItemRepository(GalaxyPvPContext db, IMapper mapper, IPlayerRepository playerRepo) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _playerRepo = playerRepo;
        }

        public async Task<ApiResponse<PlayerItemDto>> Get(int itemId)
        {
            try
            {
                var item = await FindAsync(p => p.Id == itemId);
                if (item == null)
                {
                    return ApiResponse<PlayerItemDto>.ReturnFailed(401, "Not Found!");
                }
                PlayerItemDto reponse = _mapper.Map<PlayerItemDto>(item);
                return ApiResponse<PlayerItemDto>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerItemDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerItemDto>> Create(PlayerItemCreateDto itemCreateDto)
        {
            try
            {
                if (itemCreateDto == null)
                {
                    return ApiResponse<PlayerItemDto>.ReturnFailed(401, "Create data is null");
                }
                if (await FindAsync(p => p.DataId == itemCreateDto.DataId && p.PlayerId == itemCreateDto.PlayerId) != null)
                {
                    return ApiResponse<PlayerItemDto>.ReturnFailed(401, "Item exists");
                }

                PlayerItem item = new PlayerItem();
                _mapper.Map(itemCreateDto, item);
                item.CreatedAt = DateTime.Now;
                item.UpdatedAt = DateTime.Now;
                Add(item);
                Context.SaveChanges();
                PlayerItemDto playerDTO = _mapper.Map<PlayerItemDto>(item);
                return ApiResponse<PlayerItemDto>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerItemDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerItemDto>> Update(string playerId, PlayerItemUpdateDto itemUpdateDto)
        {
            try
            {
                if (itemUpdateDto == null)
                {
                    return ApiResponse<PlayerItemDto>.ReturnFailed(401, "Update data is null");
                }

                var item = await Context.Set<PlayerItem>().FirstOrDefaultAsync(p => p.DataId == itemUpdateDto.DataId && p.PlayerId == playerId);
                if (item == null)
                {
                    return ApiResponse<PlayerItemDto>.Return404("Player not found");
                }

                _mapper.Map(itemUpdateDto, item);
                item.CreatedAt = DateTime.Now;
                item.UpdatedAt = DateTime.Now;
                await Context.SaveChangesAsync();
                PlayerItemDto itemDto = _mapper.Map<PlayerItemDto>(item);
                return ApiResponse<PlayerItemDto>.ReturnResultWith200(itemDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerItemDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<PlayerItemDto>> Delete(string playerId, int dataId)
        {
            try
            {
                PlayerItem removeItem = await FindAsync(x => x.PlayerId == playerId && x.DataId == dataId);
                PlayerItemDto responseItem = _mapper.Map<PlayerItemDto>(removeItem);
                Delete(removeItem);
                Context.SaveChanges();
                return ApiResponse<PlayerItemDto>.ReturnResultWith200(responseItem);
            }
            catch (Exception ex)
            {
                return ApiResponse<PlayerItemDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<ListPlayerItemDto>> GetAll(string playerId)
        {
            try
            {
                if (playerId == null)
                {
                    return ApiResponse<ListPlayerItemDto>.ReturnFailed(401, "PlayerId is null");
                }
                List<PlayerItem> items = await Context.Set<PlayerItem>().Where(p => p.PlayerId == playerId).ToListAsync();
                if (items == null)
                {
                    return ApiResponse<ListPlayerItemDto>.ReturnFailed(401, "Not Found!");
                }
                ListPlayerItemDto response = new ListPlayerItemDto();
                response.PlayerItems = items;
                return ApiResponse<ListPlayerItemDto>.ReturnResultWith200(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ListPlayerItemDto>.ReturnFailed(401, ex.Message);
            }
        }

        public Task<ApiResponse<ListPlayerItemDto>> GetAllByNFT(string nftId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<ListCreatePlayerItemDto>> CreateList(ListCreatePlayerItemDto playerCreateListDto)
        {
            try
            {
                foreach (var item in playerCreateListDto.PlayerItems)
                {
                    var existItem = await Context.Set<PlayerItem>().FirstOrDefaultAsync(p => p.DataId == item.DataId && p.PlayerId == playerCreateListDto.PlayerId);
                    if (existItem != null)
                    {
                        return ApiResponse<ListCreatePlayerItemDto>.Return404("Item of Player exist");
                    }

                    PlayerItem itemDto = _mapper.Map<PlayerItem>(item);

                    itemDto.CreatedAt = DateTime.Now;
                    itemDto.UpdatedAt = DateTime.Now;
                    Add(itemDto);
                    await Context.SaveChangesAsync();
                }
                return ApiResponse<ListCreatePlayerItemDto>.ReturnResultWith200(playerCreateListDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<ListCreatePlayerItemDto>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<ListUpdatePlayerItemDto>> UpdateList(ListUpdatePlayerItemDto playerUpdateDto)
        {
            try
            {
                foreach (var item in playerUpdateDto.PlayerItems)
                {
                    var existItem = await Context.Set<PlayerItem>().FirstOrDefaultAsync(p => p.DataId == item.DataId && p.PlayerId == playerUpdateDto.PlayerId);
                    if (existItem == null)
                    {
                        return ApiResponse<ListUpdatePlayerItemDto>.Return404("Item not exist");
                    }

                    _mapper.Map(item, existItem);
                    existItem.CreatedAt = DateTime.Now;
                    existItem.UpdatedAt = DateTime.Now;
                    await Context.SaveChangesAsync();
                }

                return ApiResponse<ListUpdatePlayerItemDto>.ReturnResultWith200(playerUpdateDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<ListUpdatePlayerItemDto>.ReturnFailed(401, ex.Message);
            }

        }

        public Task<ApiResponse<ListPlayerItemDto>> ValidateWallet(ListPlayerItemDto playerCreateListDto)
        {
            throw new NotImplementedException();
        }
    }
}
