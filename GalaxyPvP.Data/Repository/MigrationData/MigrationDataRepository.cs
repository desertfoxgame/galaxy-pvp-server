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
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using Microsoft.VisualBasic.FileIO;

namespace GalaxyPvP.Data
{
    public class MigrationDataRepository : GenericRepository<GalaxyUser, GalaxyPvPContext>, IMigrationDataRepository
    {
        private GalaxyPvPContext _db;
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPlayerItemRepository _playerItemRepo;

        public MigrationDataRepository(GalaxyPvPContext db, IMapper mapper, IUserRepository userRepo, IPlayerRepository playerRepo, IPlayerItemRepository playerItemRepo) : base(db)
        {
            _db = db;
            _playerRepo = playerRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _playerItemRepo = playerItemRepo;
        }

        public async Task<ApiResponse<string>> MigrationUser(MigrateUserRequestDTO request)
        {
            try
            {
                RegisterRequestDTO registerDto = _mapper.Map<RegisterRequestDTO>(request);

                string password = GenerateExtionsion.GeneratePassword(16, true, true, true);
                registerDto.UserName = registerDto.Email;
                registerDto.Password = password;

                var user = await _userRepo.Register(registerDto);
                if (!user.Success)
                {
                    return ApiResponse<string>.ReturnFailed(401, user.Errors);
                }

                Player player = _mapper.Map<Player>(request);
                var newUser = _userRepo.FindBy(x => x.Email == request.Email).FirstOrDefault();
                player.Id = request.PlayfabID;
                player.UserId = newUser.Id;

                PlayerCreateDto playerCreateDto = _mapper.Map<PlayerCreateDto>(player);

                foreach (string name in request.PlayerItems)
                {
                    int dataId = GetItemDataId(name);
                    if (dataId != 0)
                    {
                        PlayerItemCreateDto itemCreateDto = new PlayerItemCreateDto();
                        itemCreateDto.PlayerId = player.Id;
                        itemCreateDto.DataId = dataId;

                        ApiResponse<PlayerItemDto> itemResponse = await _playerItemRepo.Create(itemCreateDto);
                        if (!itemResponse.Success)
                        {
                            return ApiResponse<string>.ReturnFailed(401, itemResponse.Errors);
                        }
                    }
                }

                PlayerCreateDto playerDTO = _mapper.Map<PlayerCreateDto>(player);
                ApiResponse<PlayerDto> response = await _playerRepo.Create(playerCreateDto);
                if (response.Success)
                {
                    //    await EmailExtension.SendEmailAsync(request.Email,
                    //"Mật khẩu mới của bạn",
                    //$"Mật khẩu mới cho tài khoản của bạn là: {password}");
                    return ApiResponse<string>.ReturnResultWith200("Succeed!");
                }
                else
                {
                    return ApiResponse<string>.ReturnFailed(401, response.Errors);

                }

            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(401, ex.Message);
            }
        }

        int GetItemDataId(string name)
        {
            List<DataItemCSV> data = GetListDataItemCsv();
            foreach (DataItemCSV item in data)
            {
                if (item.Name == name)
                {
                    return item.Id;
                }
            }
            return 0;
        }

        public static List<DataItemCSV> GetListDataItemCsv()
        {
            // Get the current directory of the application
            string currentDirectory = Directory.GetCurrentDirectory() + "\\DataID - Assetsitemdata.csv";

            var data = new List<DataItemCSV>();

            using (var parser = new TextFieldParser(currentDirectory))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    var item = new DataItemCSV();

                    if (fields.Length >= 2)
                    {
                        // Assuming the first field is Id and the second field is Name
                        if (int.TryParse(fields[0], out int id))
                        {
                            item.Id = id;
                        }

                        item.Name = fields[1];
                    }

                    data.Add(item);
                }
            }

            return data;
        }
    }
}
