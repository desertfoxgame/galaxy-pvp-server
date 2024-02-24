using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;

namespace GalaxyPvP.Data
{
    public class AdminRepository : GenericRepository<Player, GalaxyPvPContext>, IAdminRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;

        public AdminRepository(GalaxyPvPContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PageResponse<AdminPlayerDTO>>> GetAllPlayer(PageRequest request)
        {
            try
            {
                var players = await Context.Set<Player>().ToListAsync();
                List<AdminPlayerDTO> listDTO = new List<AdminPlayerDTO>();
                foreach (var player in players)
                {
                    var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(x => x.Id == player.UserId);
                    AdminPlayerDTO dto = new AdminPlayerDTO
                    {
                        Id = player.Id,
                        UserId = player.UserId,
                        Email = user.Email,
                        Username = user.UserName,
                        Nickname = player.Nickname,
                    };
                    listDTO.Add(dto);
                }
                var pageList = PagedList<AdminPlayerDTO>.ToPagedList(listDTO, request.PageNumber, request.PageSize);
                PageResponse<AdminPlayerDTO> response = new PageResponse<AdminPlayerDTO>
                {
                    TotalCount = pageList.TotalCount,
                    PageSize = pageList.PageSize,
                    CurrentPage = pageList.CurrentPage,
                    TotalPages = pageList.TotalPages,
                    HasNext = pageList.HasNext,
                    HasPrev = pageList.HasPrev,
                    Data = pageList
                };

                return ApiResponse<PageResponse<AdminPlayerDTO>>.ReturnResultWith200(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<PageResponse<AdminPlayerDTO>>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<PageResponse<AdminPlayerDTO>>> SearchPlayer(PageRequest request, string searchInput)
        {
            try
            {
                var playersNickname = await Context.Set<Player>().Include(u => u.User)
                                                                .Where(x => x.Nickname.ToLower().Contains(searchInput.ToLower().Trim())
                                                                || x.User.Email.ToLower().Contains(searchInput.ToLower().Trim()) 
                                                                || x.User.UserName.ToLower().Contains(searchInput.ToLower().Trim()))
                                                        .ToListAsync();

                List<AdminPlayerDTO> listDTO = new List<AdminPlayerDTO>();
                foreach (var player in playersNickname)
                {
                    var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(x => x.Id == player.UserId);
                    AdminPlayerDTO dto = new AdminPlayerDTO
                    {
                        Id = player.Id,
                        UserId = player.UserId,
                        Email = user.Email,
                        Username = user.UserName,
                        Nickname = player.Nickname,
                    };
                    listDTO.Add(dto);
                }

                var pageList = PagedList<AdminPlayerDTO>.ToPagedList(listDTO, request.PageNumber, request.PageSize);
                PageResponse<AdminPlayerDTO> response = new PageResponse<AdminPlayerDTO>
                {
                    TotalCount = pageList.TotalCount,
                    PageSize = pageList.PageSize,
                    CurrentPage = pageList.CurrentPage,
                    TotalPages = pageList.TotalPages,
                    HasNext = pageList.HasNext,
                    HasPrev = pageList.HasPrev,
                    Data = pageList
                };

                return ApiResponse<PageResponse<AdminPlayerDTO>>.ReturnResultWith200(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<PageResponse<AdminPlayerDTO>>.ReturnFailed(404, ex.Message);
            }
        }

        public Task<ApiResponse<PlayerUpdateDto>> Update(PlayerUpdateDto playerUpdateDto)
        {
            throw new NotImplementedException();
        }


    }
}
