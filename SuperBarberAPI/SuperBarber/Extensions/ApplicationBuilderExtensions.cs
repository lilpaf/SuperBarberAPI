using Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Enums;
using Persistence.Implementations;
using StackExchange.Redis;
using SuperBarber.Extensions.DataLoaders;

namespace SuperBarber.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDataBase(this IApplicationBuilder app)
        {
            using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

            IServiceProvider services = serviceScope.ServiceProvider;

            SuperBarberDbContext data = services.GetRequiredService<SuperBarberDbContext>();
            IDatabase cache = services.GetRequiredService<IDatabase>();

            data.Database.Migrate();

            SeedCategories(data);
            SeedOrderCancellationReasons(data);
            SeedCities(data, cache);
            SeedNeighborhoods(data, cache);
            SeedWeekDays(data);
            SeedAdministrator(services);
            SeedBarberRole(services);
            SeedBarberShopOwnerRole(services);

            return app;
        }

        private static void SeedCategories(SuperBarberDbContext data)
        {
            if (data.Categories.Any())
            {
                return;
            }

            string[] categoryName = Enum.GetNames<CategoryEnum>();
            CategoryEnum[] categoryEnum = Enum.GetValues<CategoryEnum>();

            for (int i = 0; i < categoryName.Length; i++)
            {
                data.Categories.Add(new Category
                {
                    CategoryName = categoryName[i],
                    CategoryEnum = categoryEnum[i]
                });
            }

            data.SaveChanges();
        }
        
        private static void SeedOrderCancellationReasons(SuperBarberDbContext data)
        {
            if (data.OrderCancellationReasons.Any())
            {
                return;
            }

            CancellationReasonEnum[] cancellationReasonEnum = Enum.GetValues<CancellationReasonEnum>();

            for (int i = 0; i < cancellationReasonEnum.Length; i++)
            {
                data.OrderCancellationReasons.Add(new OrderCancellationReason
                {
                    Reason = cancellationReasonEnum[i].GetDisplayName(),
                    CancellationReasonEnum = cancellationReasonEnum[i]
                });
            }

            data.SaveChanges();
        }

        private static void SeedWeekDays(SuperBarberDbContext data)
        {
            if (data.WeekDays.Any())
            {
                return;
            }

            string[] weekDayName = Enum.GetNames<DayOfWeek>();
            DayOfWeek[] weekDayEnum = Enum.GetValues<DayOfWeek>();

            for (int i = 1; i < weekDayName.Length; i++)
            {
                data.WeekDays.Add(new WeekDay
                {
                    DayOfWeekName = weekDayName[i],
                    DayOfWeekEnum = weekDayEnum[i]
                });
            }

            // Sunday needs to be at the end
            data.WeekDays.Add(new WeekDay
            {
                DayOfWeekName = weekDayName[0],
                DayOfWeekEnum = weekDayEnum[0]
            });

            data.SaveChanges();
        }

        private static void SeedCities(SuperBarberDbContext data, IDatabase cache)
        {
            if (data.Cities.Any())
            {
                return;
            }

            IReadOnlyList<string> cities = DataLoader.LoadCitiesFromJson();

            foreach (var city in cities)
            {
                data.Cities.Add(new City
                {
                    Name = city
                });

                cache.ListRightPush(RedisConstants.CitiesKeyRedis, city);
            }

            data.SaveChanges();
        }

        private static void SeedNeighborhoods(SuperBarberDbContext data, IDatabase cache)
        {
            if (data.Neighborhoods.Any())
            {
                return;
            }

            Dictionary<string, IReadOnlyList<string>> citiesNeighborhoods = DataLoader.LoadNeighborhoodsFromJson();

            foreach (var cityNeighborhoods in citiesNeighborhoods)
            {
                City city = data.Cities.First(c => c.Name == cityNeighborhoods.Key);

                foreach (var neighborhood in cityNeighborhoods.Value)
                {
                    data.Neighborhoods.Add(new Neighborhood
                    {
                        Name = neighborhood,
                        CityId = city.Id
                    });

                    cache.ListRightPush(string.Format(RedisConstants.NeighborhoodsKeyRedis, cityNeighborhoods.Key), neighborhood);
                }
            }

            data.SaveChanges();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task.Run(async () =>
            {
                if (await roleManager.RoleExistsAsync(RolesConstants.AdministratorRoleName))
                {
                    return;
                }

                IdentityRole role = new () 
                { 
                    Name = RolesConstants.AdministratorRoleName 
                };

                await roleManager.CreateAsync(role);

                string adminPassword = "admin!23";

                User user = new()
                {
                    Email = "admin@barbers.com",
                    UserName = "admin@barbers.com",
                    FirstName = "AdminFirstName",
                    LastName = "AdminLastName",
                    IsDeleted = false,
                    DeleteDate = null,
                    AverageRating = 0
                };

                await userManager.CreateAsync(user, adminPassword);

                await userManager.AddToRoleAsync(user, role.Name);
            })
            .GetAwaiter()
            .GetResult();
        }

        private static void SeedBarberRole(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task.Run(async () =>
            {
                if (await roleManager.RoleExistsAsync(RolesConstants.BarberRoleName))
                {
                    return;
                }

                IdentityRole role = new () 
                { 
                    Name = RolesConstants.BarberRoleName 
                };

                await roleManager.CreateAsync(role);
            })
            .GetAwaiter()
            .GetResult();
        }

        private static void SeedBarberShopOwnerRole(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task.Run(async () =>
            {
                if (await roleManager.RoleExistsAsync(RolesConstants.BarberShopOwnerRoleName))
                {
                    return;
                }

                IdentityRole role = new () 
                { 
                    Name = RolesConstants.BarberShopOwnerRoleName 
                };

                await roleManager.CreateAsync(role);
            })
            .GetAwaiter()
            .GetResult();
        }
    }
}
