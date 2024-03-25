using AutoMapper;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Dto.User;
using GalaxyPvP.Extensions;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Data.Dto.Player;
using GalaxyPvP.Data.Model;
using Microsoft.VisualBasic.FileIO;
using GalaxyPvP.Data.Dto.MigrationDB;
using Microsoft.EntityFrameworkCore;
using Quantum;

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


        public async Task<ApiResponse<string>> AddItemData()
        {
            try
            {
                List<DataItemCSV> data = GetListDataItemCsv();
                foreach (DataItemCSV item in data)
                {
                    ItemDataMigration itemData = new ItemDataMigration();
                    itemData.DataId = item.Id;
                    itemData.Name = item.Name;
                    Context.Set<ItemDataMigration>().Add(itemData);
                }
                await Context.SaveChangesAsync();
                return ApiResponse<string>.ReturnSuccess();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<MigrateUserResponseDTO>> MigrationUser(MigrateUserRequestDTO request)
        {
            try
            {
                MigrateUserResponseDTO response = new();
                _mapper.Map(request, response);
                response.PlayerItemsCantCreate = [];

                // Create user using playfab data
                RegisterRequestDTO registerDto = _mapper.Map<RegisterRequestDTO>(request);
                string password = GenerateExtension.GeneratePassword(16);
                if (request.Developer == 1)
                    password = "a12345A!";
                registerDto.UserName = registerDto.Email;
                registerDto.Password = password;
                var user = await _userRepo.RegisterWithEmail(registerDto);
                if (!user.Success)
                    return ApiResponse<MigrateUserResponseDTO>.ReturnFailed(404, user.Errors);
                
                if (request.Developer != 1)
                    await _userRepo.ForgotPassword(request.Email); // Send reset password to email

                // Create Pvp player using playfab data
                Player player = _mapper.Map<Player>(request);
                var newUser = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(u => u.Email == request.Email);
                if (newUser != null) 
                {
                    if (request.Developer == 1)
                        newUser.EmailConfirmed = true;
                    else
                        newUser.EmailConfirmed = request.Verification == "Pending" ? false : true; // Email confirm with verification from playfab
                }
                    
                player.Id = request.PlayfabID;
                player.UserId = newUser.Id;

                // Create Player Item from playfab inventory
                foreach (string name in request.PlayerItems)
                {
                    int dataId = await GetItemDataId(name);
                    if (dataId != 0)
                    {
                        PlayerItemCreateDto itemCreateDto = new()
                        {
                            DataId = dataId
                        };

                        if (await Context.Set<PlayerItem>().FirstOrDefaultAsync(p => p.PlayerId == player.Id && p.DataId == itemCreateDto.DataId) != null)
                        {
                            response.PlayerItemsCantCreate.Add(name);
                        }
                        else
                        {
                            PlayerItem item = new ();
                            _mapper.Map(itemCreateDto, item);
                            item.PlayerId = player.Id;
                            item.CreatedAt = DateTime.Now;
                            item.UpdatedAt = DateTime.Now;
                            Context.Set<PlayerItem>().Add(item);
                        }
                    }
                    else
                        response.PlayerItemsCantCreate.Add(name);
                }
                await Context.SaveChangesAsync();
                // Create player
                ApiResponse<PlayerDto> migrateDataCreate = await _playerRepo.MigrateDataCreate(newUser.Id, player);

                if (migrateDataCreate.Success)
                    return ApiResponse<MigrateUserResponseDTO>.ReturnResultWith200(response);
                else
                    return ApiResponse<MigrateUserResponseDTO>.ReturnFailed(404, migrateDataCreate.Errors);
            }
            catch (Exception ex)
            {
                return ApiResponse<MigrateUserResponseDTO>.ReturnFailed(404, ex.Message);
            }
        }

        public async Task<ApiResponse<MigrateUserResponseDTO>> MigrationUser(MigrateUserRequestDTO request, string password)
        {
            try
            {
                MigrateUserResponseDTO response = new();
                _mapper.Map(request, response);
                response.PlayerItemsCantCreate = [];

                // Create user using playfab data
                RegisterRequestDTO registerDto = _mapper.Map<RegisterRequestDTO>(request);
                registerDto.UserName = registerDto.Email;
                registerDto.Password = password;
                var user = await _userRepo.RegisterWithEmail(registerDto);
                if (!user.Success)
                    return ApiResponse<MigrateUserResponseDTO>.ReturnFailed(404, user.Errors);

                //if (request.Developer != 1)
                //    await _userRepo.ForgotPassword(request.Email); // Send reset password to email

                // Create Pvp player using playfab data
                Player player = _mapper.Map<Player>(request);
                var newUser = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(u => u.Email == request.Email);
                if (newUser != null)
                {
                    if (request.Developer == 1)
                        newUser.EmailConfirmed = true;
                    else
                        newUser.EmailConfirmed = request.Verification == "Pending" ? false : true; // Email confirm with verification from playfab
                }

                player.Id = request.PlayfabID;
                player.UserId = newUser.Id;

                // Create Player Item from playfab inventory
                foreach (string name in request.PlayerItems)
                {
                    int dataId = await GetItemDataId(name);
                    if (dataId != 0)
                    {
                        PlayerItemCreateDto itemCreateDto = new()
                        {
                            DataId = dataId
                        };

                        if (await Context.Set<PlayerItem>().FirstOrDefaultAsync(p => p.PlayerId == player.Id && p.DataId == itemCreateDto.DataId) != null)
                        {
                            response.PlayerItemsCantCreate.Add(name);
                        }
                        else
                        {
                            PlayerItem item = new();
                            _mapper.Map(itemCreateDto, item);
                            item.PlayerId = player.Id;
                            item.CreatedAt = DateTime.Now;
                            item.UpdatedAt = DateTime.Now;
                            Context.Set<PlayerItem>().Add(item);
                        }
                    }
                    else
                        response.PlayerItemsCantCreate.Add(name);
                }
                await Context.SaveChangesAsync();
                // Create player
                ApiResponse<PlayerDto> migrateDataCreate = await _playerRepo.MigrateDataCreate(newUser.Id, player);

                if (migrateDataCreate.Success)
                    return ApiResponse<MigrateUserResponseDTO>.ReturnResultWith200(response);
                else
                    return ApiResponse<MigrateUserResponseDTO>.ReturnFailed(404, migrateDataCreate.Errors);
            }
            catch (Exception ex)
            {
                return ApiResponse<MigrateUserResponseDTO>.ReturnFailed(404, ex.Message);
            }
        }

        async Task<int> GetItemDataId(string name)
        {
            var itemData = await Context.Set<ItemDataMigration>().FirstOrDefaultAsync(x => x.Name == name);
            if (itemData == null) return 0;
            return itemData.DataId;
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

        public async Task<ApiResponse<string>> DeleteMigrationUser(string email)
        {
            try
            {
                var user = await Context.Set<GalaxyUser>().FirstOrDefaultAsync(x => x.Email == email);
                if (user == null) return ApiResponse<string>.Return404("User not found");
                Player player = await Context.Set<Player>().Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

                if (player != null)
                {
                    // Remove player item
                    List<PlayerItem> listItem = [];
                    listItem = await Context.Set<PlayerItem>().Where(x => x.PlayerId == player.Id).ToListAsync();
                    if (listItem.Count > 0)
                        Context.Set<PlayerItem>().RemoveRange(listItem);

                    // Remove player friend
                    List<Friend> listFriend = [];
                    listFriend = await Context.Set<Friend>().Where(x => x.Player1Id == player.Id || x.Player2Id == player.Id).ToListAsync();
                    if (listFriend?.Count > 0)
                        Context.Set<Friend>().RemoveRange(listFriend);

                    // Remove player
                    Context.Set<Player>().Remove(player);
                }

                // Remove User
                Context.Set<GalaxyUser>().Remove(user);
                Context.SaveChanges();

                return ApiResponse<string>.ReturnResultWith200("Successful!");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ReturnFailed(404, ex.Message);
            }
        }
    }
}
