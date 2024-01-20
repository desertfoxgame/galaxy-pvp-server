using AutoMapper;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using GalaxyPvP.Helper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;

namespace GalaxyPvP.Api.Controllers
{
    public class MigrationDataController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPlayerItemRespository _playerItemRepo;
        private readonly IMigrationDataRepository _migrationDataRepo;

        public MigrationDataController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepo, IPlayerRepository playerRepo, 
            IPlayerItemRespository playerItemRepo, IMigrationDataRepository migrationDataRepo)
        {
            _logger = logger;
            _playerRepo = playerRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _playerItemRepo = playerItemRepo;
            _migrationDataRepo = migrationDataRepo;
        }

        [HttpPost("MigrateUser")]
        public async Task<IActionResult> MigrationUser([FromBody] MigrateUserRequestDTO request)
        {
            ApiResponse<string> response = await _migrationDataRepo.MigrationUser(request);
            return ReturnFormatedResponse(response);
        }
        
    }
}
