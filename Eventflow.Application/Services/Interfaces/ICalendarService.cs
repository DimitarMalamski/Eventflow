using Eventflow.Domain.Models.DTOs;

namespace Eventflow.Application.Services.Interfaces
{
    public interface ICalendarService
    {
        public CalendarDto GenerateCalendar(int year, int month, List<PersonalEventWithCategoryNameDto> personalEvents);
        public CalendarDto GenerateCalendar(int year, int month);
        public Task<CalendarDto> GenerateUserCalendarAsync(int userId, int year, int month);
        public Task<CalendarDto> GenerateNationalHolidayCalendarAsync(int countryId, int year, int month);
        public CalendarDto GenerateEmptyCalendar(int year, int month);
    }
}
