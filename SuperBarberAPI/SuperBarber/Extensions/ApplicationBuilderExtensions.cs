using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Entities;

namespace SuperBarber.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDataBase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var services = serviceScope.ServiceProvider;

            var data = services.GetRequiredService<SuperBarberDbContext>();

            data.Database.Migrate();

            //ToDo later on implement them
            SeedCategories(data);
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
    }
}
