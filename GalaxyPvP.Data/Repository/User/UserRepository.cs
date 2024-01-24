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
using Microsoft.EntityFrameworkCore;

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

            
            if (user == default)
            {
                return ApiResponse<LoginResponseDTO>.ReturnUserNotFound();
            }
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
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
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

        public async Task<ApiResponse<UserDTO>> GetById(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "UserId Null");
                }
                var list = await Context.Set<GalaxyUser>().ToListAsync();
                var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(p => p.Id == userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(401, ex.Message);

            }
        }

        public async Task<ApiResponse<UserDTO>> GetByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "UserEmail Null");
                }
                var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(p => p.Email == email);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(401, ex.Message);

            }
        }

        public async Task<ApiResponse<UserDTO>> GetByUserName(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "UserName Null");
                }
                var user = await FindAsync(p => p.UserName == userName);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<ApiResponse<UserDTO>> Update(UserDTO userUpdateDto)
        {
            try
            {
                if (userUpdateDto == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "Update data is null");
                }

                var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(p => p.Id == userUpdateDto.ID);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Return404("Player not found");
                }

                _mapper.Map(userUpdateDto, user);
                await Context.SaveChangesAsync();
                UserDTO playerDTO = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(playerDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(401, ex.Message);
            }
        }
    }
}
