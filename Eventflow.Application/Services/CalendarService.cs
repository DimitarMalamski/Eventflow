using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IPersonalEventService _personalEventService;
        private readonly INationalEventService _nationalEventService;
        public CalendarService(IPersonalEventService personalEventService, 
            INationalEventService nationalEventService)
        {
            _personalEventService = personalEventService;
            _nationalEventService = nationalEventService;
        }
        public CalendarViewModel GenerateCalendar(int year, int month, List<PersonalEventWithCategoryNameViewModel> personalEvents)
        {
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayWeek = firstDay.DayOfWeek == 0 ? 6 : (int)firstDay.DayOfWeek - 1;

            var model = new CalendarViewModel
            {
                Year = year,
                Month = month
            };

            for (int i = 0; i < startDayWeek; i++)
            {
                model.Days.Add(new CalendarDay
                {
                    DayNumber = null,
                    PersonalEvents = new List<PersonalEventWithCategoryNameViewModel>()
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);

                bool isToday = day == DateTime.Today.Day &&
                                month == DateTime.Today.Month &&
                                year == DateTime.Today.Year;

                var eventsForThisDay = personalEvents
                    .Where(e => e.Date.Date == date.Date)
                    .ToList();

                model.Days.Add(new CalendarDay
                {
                    DayNumber = day,
                    IsToday = isToday,
                    Date = date,
                    PersonalEvents = eventsForThisDay
                });
            }

            while (model.Days.Count % 7 != 0)
            {
                model.Days.Add(new CalendarDay
                {
                    DayNumber = null,
                    PersonalEvents = new List<PersonalEventWithCategoryNameViewModel>()
                });
            }

            return model;
        }
        public CalendarViewModel GenerateCalendar(int year, int month, List<NationalEventViewModel> nationalHolidays)
        {
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayWeek = firstDay.DayOfWeek == 0 ? 6 : (int)firstDay.DayOfWeek - 1;

            var model = new CalendarViewModel
            {
                Year = year,
                Month = month
            };

            for (int i = 0; i < startDayWeek; i++)
            {
                model.Days.Add(new CalendarDay
                {
                    DayNumber = null,
                    NationalEvents = new List<NationalEventViewModel>()
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);

                bool isToday = date.Date == DateTime.Today.Date;

                var nationalForDay = nationalHolidays
                    .Where(h => h.Date.Date == date.Date)
                    .ToList();

                model.Days.Add(new CalendarDay
                {
                    DayNumber = day,
                    IsToday = isToday,
                    Date = date,
                    NationalEvents = nationalForDay
                });
            }

            while (model.Days.Count % 7 != 0)
            {
                model.Days.Add(new CalendarDay
                {
                    DayNumber = null,
                    NationalEvents = new List<NationalEventViewModel>()
                });
            }

            return model;
        }
        public CalendarViewModel GenerateCalendar(int year, int month)
        {
            return GenerateCalendar(year, month, new List<PersonalEventWithCategoryNameViewModel>());
        }
        public async Task<CalendarViewModel> GenerateNationalHolidayCalendarAsync(int countryId, int year, int month)
        {
            var nationalEvents = await _nationalEventService.GetNationalHolidaysForCountryAsync(countryId, year, month);

            Console.WriteLine($"🌍 Loaded {nationalEvents.Count} holidays for country ID {countryId}");

            return GenerateCalendar(
                year,
                month,
                nationalEvents
            );
        }
        public async Task<CalendarViewModel> GenerateUserCalendarAsync(int userId, int year, int month)
        {
            var ownEvents = await _personalEventService.GetEventsWithCategoryNamesAsync(userId, year, month);
            var invitedEvents = await _personalEventService.GetAcceptedInvitedEventsAsync(userId, year, month);
            var combinedEvents = ownEvents.Concat(invitedEvents).ToList();

            return GenerateCalendar(year, month, combinedEvents);
        }
    }
}
