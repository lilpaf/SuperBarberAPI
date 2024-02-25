using Business.Configurations;
using Business.Implementations;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using Persistence.Implementations;
using Persistence.Interfaces;
using SuperBarber.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SuperBarberDbContext>(
            optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SuperBarber")));

builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    byte[] key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWTConfig:Secret").Value) 
        ?? throw new NotConfiguredException("JWTConfig is not configured correctly or it is missing");
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = false, // ToDo fix it
        ValidateLifetime = true
    };
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddScoped<IUserService, UserService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.PrepareDataBase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
