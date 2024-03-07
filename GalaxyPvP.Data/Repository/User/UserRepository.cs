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
using Azure.Core;
using GalaxyPvP.Data.Model;
using System.Text.RegularExpressions;
using Microsoft.AspNet.SignalR;
using GalaxyPvP.Data.Dto;

namespace GalaxyPvP.Data.Repository.User
{
    public class UserRepository : GenericRepository<GalaxyUser, GalaxyPvPContext>, IUserRepository
    {
        private readonly UserManager<GalaxyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;
        private enum InfoType { email, username, wallet };

        public UserRepository(GalaxyPvPContext db, IConfiguration configuration,
            UserManager<GalaxyUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager) : base(db)
        {
            _userManager = userManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = Context.GalaxyUsers.FirstOrDefault(x => x.UserName == username);
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
                return ApiResponse<LoginResponseDTO>.ReturnFailed(404, "UserName Or Password is InCorrect.");
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
                    new Claim("UserName", user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            Player player = Context.Set<Player>().FirstOrDefault(x => x.UserId == user.Id);
            List<PlayerItem> playerItems = new List<PlayerItem>();
            if (player != null)
            {
                playerItems = Context.Set<PlayerItem>().Where(x => x.PlayerId == player.Id).ToList();
            }

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                Role = roles.FirstOrDefault(),
            };

            user.Token = loginResponseDTO.Token;
            await Context.SaveChangesAsync();

            if (player != null)
            {
                loginResponseDTO.Player = _mapper.Map<PlayerDto>(player);
                loginResponseDTO.Player.WalletAddress = user.WalletAddress;
            }

            loginResponseDTO.Items = playerItems;
            return ApiResponse<LoginResponseDTO>.ReturnResultWith200(loginResponseDTO);
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginWithWallet(LoginRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.WalletAddress))
                return ApiResponse<LoginResponseDTO>.ReturnFailed(404, "Wallet address invalid.");

            var user = Context.GalaxyUsers.FirstOrDefault(u => u.WalletAddress == request.WalletAddress);
            if (user == null)
                return ApiResponse<LoginResponseDTO>.ReturnFailed(404, "Wallet address is InCorrect.");
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim("UserName", user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            Player player = Context.Set<Player>().FirstOrDefault(x => x.UserId == user.Id);
            List<PlayerItem> playerItems = new List<PlayerItem>();
            if (player != null)
            {
                playerItems = Context.Set<PlayerItem>().Where(x => x.PlayerId == player.Id).ToList();
            }

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                Role = roles.FirstOrDefault(),
            };

            user.Token = loginResponseDTO.Token;
            await Context.SaveChangesAsync();

            if (player != null)
            {
                loginResponseDTO.Player = _mapper.Map<PlayerDto>(player);
                loginResponseDTO.Player.WalletAddress = user.WalletAddress;
            }

            loginResponseDTO.Items = playerItems;
            return ApiResponse<LoginResponseDTO>.ReturnResultWith200(loginResponseDTO);
        }

        public async Task<ApiResponse<UserDTO>> RegisterWithEmail(RegisterRequestDTO request)
        {
            if (!IsValidEmail(request.Email))
                return ApiResponse<UserDTO>.ReturnFailed(400, "Invalid Email.");

            if (string.IsNullOrEmpty(request.Password))
                return ApiResponse<UserDTO>.Return409("Password is Empty.");

            if (await UserExists(request.Email, InfoType.email))
                return ApiResponse<UserDTO>.Return409("Email is already in use");

            if (await UserExists(request.UserName, InfoType.username))
                return ApiResponse<UserDTO>.Return409("Username is already in use");

            if (await UserExists(request.WalletAddress, InfoType.wallet))
                return ApiResponse<UserDTO>.Return409("Wallet address is already in use");
            

            GalaxyUser entity = new()
            {
                UserName = request.UserName,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                PlayfabId = request.PlayfabId,
                WalletAddress = request.WalletAddress
            };

            IdentityResult result = await _userManager.CreateAsync(entity, request.Password);

            if (!result.Succeeded)
            {
                string error = "";
                foreach(IdentityError e in result.Errors)
                {
                    error += e.Description + "\n";
                }
                return ApiResponse<UserDTO>.ReturnFailed(400, error);
            }

            var newUser = await _userManager.FindByEmailAsync(request.Email);

            await SendVerifyCode(request.Email, newUser.Id);

            if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            if (!_roleManager.RoleExistsAsync("moderator").GetAwaiter().GetResult())
                await _roleManager.CreateAsync(new IdentityRole("moderator"));
            if (!_roleManager.RoleExistsAsync("player").GetAwaiter().GetResult())
                await _roleManager.CreateAsync(new IdentityRole("player"));

            await _userManager.AddToRoleAsync(entity, "player");
            await Context.SaveChangesAsync();
            return ApiResponse<UserDTO>.ReturnResultWith200(_mapper.Map<UserDTO>(entity));
        }

        public async Task<ApiResponse<UserDTO>> RegisterWithWallet(RegisterRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.WalletAddress))
                return ApiResponse<UserDTO>.ReturnFailed(400, "Invalid wallet address.");

            if (await UserExists(request.WalletAddress, InfoType.wallet))
                return ApiResponse<UserDTO>.ReturnFailed(400, "Wallet address has been registered to the account.");

            string username = request.WalletAddress.Substring(0, 6) + "..."
                + request.WalletAddress.Substring(request.WalletAddress.Length - 4,4);
            GalaxyUser entity = new()
            {
                UserName = username,
                WalletAddress = request.WalletAddress
            };
            IdentityResult result = await _userManager.CreateAsync(entity);
            if (!result.Succeeded)
            {
                string error = "";
                foreach (IdentityError e in result.Errors)
                {
                    error += e.Description + "\n";
                }
                return ApiResponse<UserDTO>.ReturnFailed(400, error);
            }

            if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            if (!_roleManager.RoleExistsAsync("moderator").GetAwaiter().GetResult())
                await _roleManager.CreateAsync(new IdentityRole("moderator"));
            if (!_roleManager.RoleExistsAsync("player").GetAwaiter().GetResult())
                await _roleManager.CreateAsync(new IdentityRole("player"));

            await _userManager.AddToRoleAsync(entity, "player");

            return ApiResponse<UserDTO>.ReturnResultWith200(_mapper.Map<UserDTO>(entity));
        }

        static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        public async Task<ApiResponse<string>> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return ApiResponse<string>.Return409("Email is Empty.");
            }
            try
            {
                GalaxyUser user = await Context.Users.FirstOrDefaultAsync(x => x.Email == email.Trim());
                if (user == null)
                    return ApiResponse<string>.Return409("Email user not exist!");

                await SendVerifyCode(email, user.Id);
                await Context.SaveChangesAsync();
                return ApiResponse<string>.ReturnResultWith200("Success!");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> ResetPassword(string verifyCode, string newPassword)
        {
            try
            {
                VerifyCode userCode = await Context.VerifyCodes.FirstOrDefaultAsync(x => x.Code == verifyCode);
                if (userCode == null)
                    return ApiResponse<string>.Return409("Verify Code not exists!");
                GalaxyUser user = await Context.Users.FirstOrDefaultAsync(x => x.Id == userCode.UserId);
                string password = newPassword;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
                Context.VerifyCodes.Remove(userCode);
                await Context.SaveChangesAsync();
                return ApiResponse<string>.ReturnResultWith200("Success!");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<UserDTO>> GetById(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "UserId Null");
                }
                var list = await Context.Set<GalaxyUser>().ToListAsync();
                var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(p => p.Id == userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(404, ex.Message);

            }
        }

        public async Task<ApiResponse<UserDTO>> GetByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "UserEmail Null");
                }
                var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(p => p.Email == email);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<UserDTO>> GetByUserName(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "UserName Null");
                }
                var user = await FindAsync(p => p.UserName == userName);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<UserDTO>> Update(UserDTO userUpdateDto)
        {
            try
            {
                if (userUpdateDto == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "Update data is null");
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
                return ApiResponse<UserDTO>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<UserDTO>> AuthorizeUser(string userId, string token)
        {
            try
            {
                if (userId == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "UserId Null");
                }
                GalaxyUser? user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "User not exist!");
                }
                if (token == user.Token)
                {
                    return ApiResponse<UserDTO>.ReturnSuccess();
                }
                else
                {
                    return ApiResponse<UserDTO>.ReturnFailed(401, "UnAuthorized");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(401, ex.Message);
            }
        }

        public async Task<bool> IsAdminByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // User not found
                    return false;
                }

                return await _userManager.IsInRoleAsync(user, "Admin");
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> IsAdminByUserId(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // User not found
                    return false;
                }

                return await _userManager.IsInRoleAsync(user, "Admin");
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ApiResponse<UserDTO>> GetByPlayerId(string playerId)
        {
            try
            {
                if (string.IsNullOrEmpty(playerId))
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "PlayerId Null");
                }
                var player = await Context.Set<Player>().FirstOrDefaultAsync(x => x.Id == playerId);

                var user = await FindAsync(p => p.Id == player.UserId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.ReturnFailed(404, "Not Found!");
                }
                UserDTO reponse = _mapper.Map<UserDTO>(user);
                return ApiResponse<UserDTO>.ReturnResultWith200(reponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> Verificantion(string verifycode)
        {
            try
            {
                var userCode = await Context.VerifyCodes.FirstOrDefaultAsync(x => x.Code == verifycode);
                if (userCode == null)
                    return ApiResponse<string>.Return409("Verify Code not exists!");
                GalaxyUser user = await Context.Users.FirstOrDefaultAsync(x => x.Id == userCode.UserId);
                user.EmailConfirmed = true;
                Context.VerifyCodes.Remove(userCode);
                await Context.SaveChangesAsync();
                return ApiResponse<string>.ReturnResultWith200("Success!");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> UpdateWalletToUser(UpdateUserWalletDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                    return ApiResponse<string>.ReturnFailed(400, "Invalid Email.");
                if (await UserExists(request.Wallet, InfoType.wallet))
                    return ApiResponse<string>.ReturnFailed(400, "Wallet address has been registered to another account.");

                var user = Context.GalaxyUsers.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());
                if (user == default)
                    return ApiResponse<string>.ReturnFailed(400, "Invalid Email.");

                user.WalletAddress = request.Wallet;
                await Context.SaveChangesAsync();
                return ApiResponse<string>.ReturnResultWith200("Success!");
            } catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> UpdateEmailToUser(UpdateUserWalletDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Wallet))
                    return ApiResponse<string>.ReturnFailed(400, "Invalid wallet address.");
                if (await UserExists(request.Email, InfoType.email))
                    return ApiResponse<string>.ReturnFailed(400, "Email has been registered to another account.");

                var user = Context.GalaxyUsers.FirstOrDefault(u => u.WalletAddress == request.Wallet);
                if (user == default)
                    return ApiResponse<string>.ReturnFailed(400, "Invalid wallet address.");

                await SendVerifyCode(request.Email, user.Id);
                await Context.SaveChangesAsync();
                return ApiResponse<string>.ReturnResultWith200("Success!");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        private async Task<bool> UserExists(string info, InfoType infoType)
        {
            if (info.IsNullOrEmpty()) return false;
            return infoType switch
            {
                InfoType.email => await _userManager.FindByEmailAsync(info) != null,
                InfoType.username => await _userManager.FindByNameAsync(info) != null,
                InfoType.wallet => await Context.Users.FirstOrDefaultAsync(x => x.WalletAddress == info) != null,
                _ => false,
            };
        }

        private async Task SendVerifyCode(string email, string userId)
        {
            VerifyCode existCode = await Context.VerifyCodes.FirstOrDefaultAsync(x => x.UserId == userId);
            string verifyCode = GenerateExtension.GenerateID(16);

            if (existCode == null)
            {
                VerifyCode userCode = new VerifyCode();
                userCode.UserId = userId;
                userCode.Code = verifyCode;
                Context.VerifyCodes.Add(userCode);
            }
            else
            {
                existCode.Code = verifyCode;
            }
            await EmailExtension.SendGridEmailAsync(email, "Verify Code", verifyCode);
        }
    }
}
