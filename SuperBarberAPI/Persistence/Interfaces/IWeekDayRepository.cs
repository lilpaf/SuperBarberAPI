using Persistence.Entities;

namespace Persistence.Interfaces
{
    public interface IWeekDayRepository
    {
        Task<WeekDay?> GetWeekDayByDayNameAsync(string day);
    }
}
