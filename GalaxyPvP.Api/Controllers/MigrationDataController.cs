using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyPvP.Api.Controllers
{
    public class MigrationDataController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepo;
        private readonly IUserRepository _userRepo;

        public MigrationDataController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepo, IPlayerRepository playerRepo)
        {
            _logger = logger;
            _playerRepo = playerRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpPost("MigrateUser")]
        public async Task<ApiResponse<string>> MigrationUser([FromBody] MigrateUserRequestDTO request)
        {
            try
            {
                RegisterRequestDTO registerDto = _mapper.Map<RegisterRequestDTO>(request);

                string password = GenerateExtionsion.GeneratePassword(16, true, true, true);
                registerDto.UserName = registerDto.Email;
                registerDto.Password = password;

                var user = await _userRepo.Register(registerDto);
                if(!user.Success)
                {
                    return ApiResponse<string>.ReturnFailed(401, user.Errors);
                }

                Player player = _mapper.Map<Player>(request);
                _playerRepo.Add(player);
                PlayerDto playerDTO = _mapper.Map<PlayerDto>(player);

                await EmailExtension.SendEmailAsync(request.Email,
            "Mật khẩu mới của bạn",
            $"Mật khẩu mới cho tài khoản của bạn là: {password}");
                return ApiResponse<string>.ReturnSuccess();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
