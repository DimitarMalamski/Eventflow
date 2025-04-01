using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface ICalendarService
    {
        public CalendarViewModel GenerateCalendar(int year, int month);
    }
}
