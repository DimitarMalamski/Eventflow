using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Services.Interfaces
{
    public interface ICalendarService
    {
        public CalendarViewModel GenerateCalendar(int year, int month);
    }
}
