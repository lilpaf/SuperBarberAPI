using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Entities;
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

            //ToDo later on implement them
            SeedCategories(data);
            SeedCities(data, cache);
            SeedNeighborhoods(data, cache);
            //SeedAdministrotor(services);
            //SeedBarberRole(services);
            //SeedBarberShopOwnerRole(services);

            return app;
        }

        private static void SeedCategories(SuperBarberDbContext data)
        {
            if (data.Categories.Any())
            {
                return;
            }

            data.Categories.AddRange(new[]
            {
                new Category{ Name = "Hair" },
                new Category{ Name = "Face" }
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

                cache.ListRightPush(CityRepository.CitiesKeyRedis, city);
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

                    cache.ListRightPush(cityNeighborhoods.Key, neighborhood);
                }
            }

            data.SaveChanges();
        }
    }
}
