using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Persistence.Implementations
{
    public class WeekDayRepository : IWeekDayRepository
    {
        private readonly SuperBarberDbContext _context;
        private readonly ILogger<WeekDayRepository> _logger;

        public WeekDayRepository(
            SuperBarberDbContext context,
            ILogger<WeekDayRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WeekDay?> GetWeekDayByDayNameAsync(string day)
        {
            _logger.LogInformation("Getting week day from SQL Db");

            return await _context.WeekDays.FirstOrDefaultAsync(d => d.DayOfWeekName == day);
        }
    }
}
