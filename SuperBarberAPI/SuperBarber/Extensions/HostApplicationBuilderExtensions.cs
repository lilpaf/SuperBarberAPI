using Business.Models.Exceptions;
using Common.Configurations;
using Common.Constants;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using Persistence.Entities;
using Serilog;
using StackExchange.Redis;
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

            TokenValidationParameters tokenValidationParams = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                RefreshBeforeValidation = true,
                ClockSkew = TimeSpan.Zero,
            };

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(x =>
            {
                x.Cookie.Name = AuthenticationConstants.AccessTokenCookieKey;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParams;
                jwt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[AuthenticationConstants.AccessTokenCookieKey];
                        return Task.CompletedTask;
                    }
                };
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

        public static IHostApplicationBuilder AddRedisCache(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton(_ =>
            {
                string connectionString = builder.Configuration.GetConnectionString("RedisCache") ??
                                           throw new NotConfiguredException("Redis connection string missing");

                return ConnectionMultiplexer.Connect(connectionString);
            });

            builder.Services.AddScoped(provider =>
            {
                ConnectionMultiplexer multiplexer = provider.GetService<ConnectionMultiplexer>() ??
                     throw new NotConfiguredException("Redis multiplexer does not exists");
                return multiplexer.GetDatabase();
            });

            return builder;
        }

        public static IHostApplicationBuilder AddKafkaProducerSingleton(this IHostApplicationBuilder builder)
        {
            KafkaProducerConfig kafkaConfig = builder.Configuration.GetSection(nameof(KafkaProducerConfig)).Get<KafkaProducerConfig>() ??
                    throw new NotConfiguredException("The kafka producer config is not configured correctly");

            ProducerConfig producerConfig = new()
            {
                BootstrapServers = kafkaConfig.BootstrapServers,
                ClientId = kafkaConfig.ClientId
            };

            builder.Services.AddSingleton(provider =>
            {
                return new ProducerBuilder<Null, string>(producerConfig).Build();
            });

            return builder;
        }

        public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
        {
            Console.OutputEncoding = Encoding.UTF8;

            builder.Host.UseSerilog((hostContext, service, configuration) =>
            {
                configuration.ReadFrom.Configuration(builder.Configuration);
            });

            return builder;
        }
    }
}
