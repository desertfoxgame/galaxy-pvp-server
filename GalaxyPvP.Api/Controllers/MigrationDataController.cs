using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.MigrationDB;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationDataController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPlayerItemRepository _playerItemRepo;
        private readonly IMigrationDataRepository _migrationDataRepo;

        public MigrationDataController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepo, IPlayerRepository playerRepo, 
            IPlayerItemRepository playerItemRepo, IMigrationDataRepository migrationDataRepo)
        {
            _logger = logger;
            _playerRepo = playerRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _playerItemRepo = playerItemRepo;
            _migrationDataRepo = migrationDataRepo;
        }

        [HttpPost("AddItemData")]
        public async Task<IActionResult> AddItemData()
        {
            ApiResponse<string> response = await _migrationDataRepo.AddItemData();
            return ReturnFormatedResponse(response);
        }

        [HttpPost("MigrateUser")]
        public async Task<IActionResult> MigrationUser([FromBody] MigrateUserRequestDTO request)
        {
            ApiResponse<MigrateUserResponseDTO> response = await _migrationDataRepo.MigrationUser(request);
            return ReturnFormatedResponse(response);
        }

        [HttpDelete("DeleteMigrateUser")]
        public async Task<IActionResult> DeleteMigrateUser(string playerId)
        {
            ApiResponse<string> response = await _migrationDataRepo.DeleteMigrationUser(playerId);
            return ReturnFormatedResponse(response);
        }
    }
}
