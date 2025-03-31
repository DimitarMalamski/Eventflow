using Eventflow.Application.Services.Interfaces;

namespace Eventflow.Application.Services
{
    public class CalendarNavigationService : ICalendarNavigationService
    {
        public (int year, int month) Normalize(int? year, int? month)
        {
            int currentYear = year ?? DateTime.Now.Year;
            int currentMonth = month ?? DateTime.Now.Month;

            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            else if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }

            return (currentYear, currentMonth);
        }
    }
}
