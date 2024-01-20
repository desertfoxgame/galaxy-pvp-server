using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Data.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;
using GalaxyPvP.Extensions;

namespace GalaxyPvP.Data.Repository.User
{
    public class UserRepository : GenericRepository<GalaxyUser, GalaxyPvPContext>, IUserRepository
    {
        private readonly UserManager<GalaxyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;

        public UserRepository(GalaxyPvPContext db, IConfiguration configuration,
            UserManager<GalaxyUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager):base(db)
        {
            _userManager = userManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = Context.GalaxyUsers.FirstOrDefault(x=>x.UserName == username);
            if (user == null) 
                return true;
            return false;
        }

        public async Task<ApiResponse<LoginResponseDTO>> Login(LoginRequestDTO request)
        {
            var user = Context.GalaxyUsers.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (user == null || isValid == false)
            {
                return ApiResponse<LoginResponseDTO>.ReturnFailed(401, "UserName Or Password is InCorrect.");
            }

            //if user was found generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                Role = roles.FirstOrDefault(),
            };
            return ApiResponse<LoginResponseDTO>.ReturnResultWith200(loginResponseDTO);
        }

        public async Task<ApiResponse<UserDTO>> Register(RegisterRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Password))
            {
                return ApiResponse<UserDTO>.Return409("Password is Empty.");
            }
            var existUser = await _userManager.FindByEmailAsync(request.Email);
            if(existUser != null)
            {
                return ApiResponse<UserDTO>.Return409("Email is already exist.");
            }

            GalaxyUser entity = new()
            {
                UserName = request.UserName,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),    
            };

            IdentityResult result = await _userManager.CreateAsync(entity, request.Password);
            if (!result.Succeeded)
            {
                return ApiResponse<UserDTO>.Return500();
            }
            if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
                await _roleManager.CreateAsync(new IdentityRole("moderator"));
                await _roleManager.CreateAsync(new IdentityRole("player"));
            }
            if (!_roleManager.RoleExistsAsync("player").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("player"));
            }
            await _userManager.AddToRoleAsync(entity, "player");

            return ApiResponse<UserDTO>.ReturnResultWith200(_mapper.Map<UserDTO>(entity));
        }
    }
}
