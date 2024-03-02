using Business.Configurations;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using Persistence.Entities;
using System.Text;

namespace SuperBarber.Extensions
{
    public static class HostApplicationBuilderExtensions
    {
        public static IHostApplicationBuilder AddCustomAuthentication(this IHostApplicationBuilder builder)
        {
            JwtConfig jwtConfig = builder.Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>() ??
                    throw new NotConfiguredException("The JWT config is not configured correctly");

            IdentityConfig identityConfig = builder.Configuration.GetSection(nameof(IdentityConfig)).Get<IdentityConfig>() ??
                    throw new NotConfiguredException("The identity config is not configured correctly");

            byte[] key = Encoding.UTF8.GetBytes(jwtConfig.Secret);

            TokenValidationParameters tokenValidationParams = new ()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience
            };

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParams;
            });

            builder.Services.AddDefaultIdentity<User>(options =>
            {
                //options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;  //ToDo fix it
                options.Password.RequireLowercase = false; //ToDo fix it
                options.Password.RequireNonAlphanumeric = false; //ToDo fix it
                options.Password.RequireUppercase = false; //ToDo fix it
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityConfig.LockoutMinutesTimeSpan);
                options.Lockout.MaxFailedAccessAttempts = identityConfig.MaxFailedAccessAttempts;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<SuperBarberDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddSingleton(tokenValidationParams);

            builder.Services.Configure<DataProtectionTokenProviderOptions>(
                options => options.TokenLifespan = TimeSpan.FromHours(identityConfig.DataProtectionTokenHoursLifetime));

            return builder;
        }
    }
}
