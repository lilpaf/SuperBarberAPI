using Business.Configurations;
using Business.Implementations;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Implementations;
using Persistence.Interfaces;
using SuperBarber.Extensions;
using SuperBarber.Filters;
using SuperBarber.Middlewares;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SuperBarberDbContext>(
            optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SuperBarber")));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    string keyValue = builder.Configuration.GetSection("JwtConfig:Secret").Value ??
        throw new NotConfiguredException("JwtConfig is not configured correctly or it is missing");
    
    byte[] key = Encoding.UTF8.GetBytes(keyValue);
  
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

builder.Services.AddDefaultIdentity<User>(options =>
{
    //ToDo uncomment this when you want to use the mailing service
    //options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;  //ToDo fix it
    options.Password.RequireLowercase = false; //ToDo fix it
    options.Password.RequireNonAlphanumeric = false; //ToDo fix it
    options.Password.RequireUppercase = false; //ToDo fix it
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<SuperBarberDbContext>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
                //This is needed in order to use the ValidateModelStateFilter
                options.SuppressModelStateInvalidFilter = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddScoped<IUserService, UserService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Filters
builder.Services.AddMvc(options =>
{
    options.Filters.Add<ValidateModelStateFilter>(0);
});

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

// Middlewares
app.UseMiddleware<LogMetaDataMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();


