using Business.Interfaces;
using Business.Models.Responses;
using Microsoft.Extensions.Logging;
using Persistence.Dtos;
using Persistence.Interfaces;

namespace Business.Implementations
{
    public class BarberShopService : IBarberShopService
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ILogger<UserService> _logger;

        public BarberShopService(
            ILogger<UserService> logger, 
            IBarberShopRepository barberShopRepository)
        {
            _logger = logger;
            _barberShopRepository = barberShopRepository;
        }

        public async Task<AllBarberShopsResponse> GetAllBarberShopsAsync()
        {
            IReadOnlyList<BarberShopDto> activeBarberShops = await _barberShopRepository.GetAllActiveBarberShopsAsync();

            return new AllBarberShopsResponse()
            { 
                BarberShops = activeBarberShops
            };
        }
    }
}
