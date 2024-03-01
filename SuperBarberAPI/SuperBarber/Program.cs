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
using BarberShopService = Business.Implementations.BarberShopService;

var builder = WebApplication.CreateBuilder(args);

/*
 * ToDo check if email is confirmed before making orders or registering as barber
 */
// Add services to the container.

builder.Services.AddDbContext<SuperBarberDbContext>(
            optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SuperBarber")));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection(nameof(SmtpConfig)));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    JwtConfig jwtConfig = builder.Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>() ??
        throw new NotConfiguredException("The JWT config is not configured correctly");
    
    byte[] key = Encoding.UTF8.GetBytes(jwtConfig.Secret);
  
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = false, // ToDo fix it
        ValidateLifetime = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
    };
});

builder.Services.AddDefaultIdentity<User>(options =>
{
    //ToDo uncomment this when you want to use the mailing service
    //options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;  //ToDo fix it
    options.Password.RequireLowercase = false; //ToDo fix it
    options.Password.RequireNonAlphanumeric = false; //ToDo fix it
    options.Password.RequireUppercase = false; //ToDo fix it
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 7;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<SuperBarberDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(
    options => options.TokenLifespan = TimeSpan.FromHours(3));

builder.Services.AddHttpContextAccessor();

//This is needed in order to use the ValidateModelStateFilter
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBarberShopService, BarberShopService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBarberShopRepository, BarberShopRepository>();

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


