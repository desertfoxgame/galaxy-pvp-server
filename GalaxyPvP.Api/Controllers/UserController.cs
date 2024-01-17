using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;

namespace GalaxyPvP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepo;

        public UserController(ILogger<UserController> logger, IUserRepository userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            ApiResponse<LoginResponseDTO> loginResponse = await _userRepo.Login(model);
            return ReturnFormatedResponse(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            var user = await _userRepo.Register(model);
            return ReturnFormatedResponse(user);
        }
    }
}
