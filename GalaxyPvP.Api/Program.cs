using GalaxyPvP.Api.Helpers.Mapping;
using GalaxyPvP.Api.Services;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Repository.Player;
using GalaxyPvP.Data.Repository.User;
using GalaxyPvP.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayFab;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/////////////LOG
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("log/debugLogs.txt", rollingInterval:RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//////////DATABASE
builder.Services.AddDbContext<GalaxyPvPContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PvPConnection"));
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerItemRepository, PlayerItemRepository>();
builder.Services.AddScoped<IMigrationDataRepository, MigrationDataRepository>();
builder.Services.AddScoped<IGameConfigRepository, GameConfigRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<UserInfoToken>(c => new UserInfoToken() { Id = "" });

//builder.Services.AddIdentity<GalaxyUser, IdentityRole>().AddEntityFrameworkStores<GalaxyPvPContext>();
builder.Services.AddIdentity<GalaxyUser, IdentityRole>()
    .AddEntityFrameworkStores<GalaxyPvPContext>()
    .AddDefaultTokenProviders();
builder.Services.AddResponseCaching();

////////AUTO MAPPING
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

JwtSettings settings = new JwtSettings() 
{
    Key = builder.Configuration.GetValue<string>("ApiSettings:Secret")
};
builder.Services.AddJwtAutheticationConfiguration(settings);

////////////SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<MatchMakingBackgroundService>();

PlayFabSettings.staticSettings.TitleId = "903AC";
//PlayFabSettings.staticSettings.DeveloperSecretKey = "I8BHKCU9NFZPM1NF74PNCGB4WPYM8HEMOSOZGHW6XSIT9PKY5B";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
