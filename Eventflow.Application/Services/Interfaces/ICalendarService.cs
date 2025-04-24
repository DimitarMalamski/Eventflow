using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface ICalendarService
    {
        public CalendarViewModel GenerateCalendar(int year, int month, List<PersonalEventWithCategoryNameViewModel> personalEvents);
        public CalendarViewModel GenerateCalendar(int year, int month);
        public Task<CalendarViewModel> GenerateUserCalendarAsync(int userId, int year, int month);
        public Task<CalendarViewModel> GenerateNationalHolidayCalendarAsync(int countryId, int year, int month);
    }
}
