using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Repository.Player
{
    public class PlayerItemRespository : GenericRepository<PlayerItem, GalaxyPvPContext>, IPlayerItemRespository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public PlayerItemRespository(GalaxyPvPContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
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
                //PlayerItem item = _mapper.Map<PlayerItem>(itemCreateDto);
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

        public async Task<ApiResponse<PlayerItemDto>> Update(PlayerItemDto itemUpdateDto)
        {
            try
            {
                if (itemUpdateDto == null)
                {
                    return ApiResponse<PlayerItemDto>.ReturnFailed(401, "Update data is null");
                }

                //Player player = await FindAsync(p => p.Id == playerUpdateDto.Id);
                var item = await Context.Set<PlayerItem>().FirstOrDefaultAsync(p => p.Id == itemUpdateDto.Id);
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

        public async Task<ApiResponse<PlayerItemDto>> Delete(int itemId)
        {
            try
            {
                PlayerItem removeItem = await FindAsync(x => x.Id == itemId);
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
    }
}
