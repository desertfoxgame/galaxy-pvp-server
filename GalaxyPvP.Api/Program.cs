using GalaxyPvP.Api.Helpers.Mapping;
using GalaxyPvP.Data;
using GalaxyPvP.Data.Context;
using GalaxyPvP.Data.Repository.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

builder.Services.AddIdentity<GalaxyUser, IdentityRole>().AddEntityFrameworkStores<GalaxyPvPContext>();

builder.Services.AddResponseCaching();
builder.Services.AddScoped<IUserRepository, UserRepository>();

////////AUTO MAPPING
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers();


////////////SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
