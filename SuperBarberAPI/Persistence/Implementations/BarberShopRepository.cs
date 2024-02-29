using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Dtos;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class BarberShopRepository : IBarberShopRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<BarberShopRepository> _logger;

        public BarberShopRepository(
            SuperBarberDbContext context,
            ILogger<BarberShopRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        //ToDo add query later on
        public async Task<IReadOnlyList<BarberShopDto>> GetAllActiveBarberShopsAsync()
        {
            _logger.LogInformation("Getting all barbershops");

            return await _context.BarberShops
                .Where(b => b.IsPublic && !b.IsDeleted)
                .Select(b => new BarberShopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    City = b.City.Name,
                    District = b.District.Name,
                    Street = b.Street,
                    StartHour = b.StartHour.ToString(@"hh\:mm"),
                    FinishHour = b.FinishHour.ToString(@"hh\:mm"),
                    ImageName = b.ImageName
                })
                .ToListAsync();
        }

        public async Task AddBarberShopAsync(BarberShop barberShop)
        {
            _logger.LogInformation("Adding barbershop");
            await _context.BarberShops.AddAsync(barberShop);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving barbershop");
            await _context.SaveChangesAsync();
        }
    }
}
