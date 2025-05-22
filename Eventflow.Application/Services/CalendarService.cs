using Eventflow.Application.Services.Interfaces;
using Eventflow.DTOs.DTOs;

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
        public CalendarDto GenerateCalendar(int year, int month, List<PersonalEventWithCategoryNameDto> personalEvents)
        {
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayWeek = firstDay.DayOfWeek == 0 ? 6 : (int)firstDay.DayOfWeek - 1;

            var model = new CalendarDto
            {
                Year = year,
                Month = month
            };

            for (int i = 0; i < startDayWeek; i++)
            {
                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = null,
                    PersonalEvents = new List<PersonalEventWithCategoryNameDto>()
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

                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = day,
                    IsToday = isToday,
                    Date = date,
                    PersonalEvents = eventsForThisDay
                });
            }

            while (model.Days.Count % 7 != 0)
            {
                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = null,
                    PersonalEvents = new List<PersonalEventWithCategoryNameDto>()
                });
            }

            return model;
        }
        public CalendarDto GenerateCalendar(int year, int month, List<NationalEventDto> nationalHolidays)
        {
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayWeek = firstDay.DayOfWeek == 0 ? 6 : (int)firstDay.DayOfWeek - 1;

            var model = new CalendarDto
            {
                Year = year,
                Month = month
            };

            for (int i = 0; i < startDayWeek; i++)
            {
                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = null,
                    NationalEvents = new List<NationalEventDto>()
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);

                bool isToday = date.Date == DateTime.Today.Date;

                var nationalForDay = nationalHolidays
                    .Where(h => h.Date.Date == date.Date)
                    .ToList();

                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = day,
                    IsToday = isToday,
                    Date = date,
                    NationalEvents = nationalForDay
                });
            }

            while (model.Days.Count % 7 != 0)
            {
                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = null,
                    NationalEvents = new List<NationalEventDto>()
                });
            }

            return model;
        }
        public CalendarDto GenerateCalendar(int year, int month)
        {
            return GenerateCalendar(year, month, new List<PersonalEventWithCategoryNameDto>());
        }
        public CalendarDto GenerateEmptyCalendar(int year, int month)
        {
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)firstDay.DayOfWeek - 1;

            var model = new CalendarDto
            {
                Year = year,
                Month = month
            };

            for (int i = 0; i < startDayWeek; i++)
            {
                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = null,
                    Date = null,
                    IsToday = false,
                    NationalEvents = new List<NationalEventDto>(),
                    PersonalEvents = new List<PersonalEventWithCategoryNameDto>()
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);

                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = day,
                    Date = date,
                    IsToday = date.Date == DateTime.Today.Date,
                    NationalEvents = new List<NationalEventDto>(),
                    PersonalEvents = new List<PersonalEventWithCategoryNameDto>()
                });
            }

            while (model.Days.Count % 7 != 0)
            {
                model.Days.Add(new CalendarDayDto
                {
                    DayNumber = null,
                    Date = null,
                    IsToday = false,
                    NationalEvents = new List<NationalEventDto>(),
                    PersonalEvents = new List<PersonalEventWithCategoryNameDto>()
                });
            }

            return model;
        }
        public async Task<CalendarDto> GenerateNationalHolidayCalendarAsync(int countryId, int year, int month)
        {
            var nationalEvents = await _nationalEventService.GetNationalHolidaysForCountryAsync(countryId, year, month);

            return GenerateCalendar(
                year,
                month,
                nationalEvents
            );
        }
        public async Task<CalendarDto> GenerateUserCalendarAsync(int userId, int year, int month)
        {
            var ownEvents = await _personalEventService.GetEventsWithCategoryNamesAsync(userId, year, month);
            var invitedEvents = await _personalEventService.GetAcceptedInvitedEventsAsync(userId, year, month);
            var globalEvents = await _personalEventService.GetGlobalEventsWithCategoryAsync(year, month);

            var combinedEvents = ownEvents
                .Concat(invitedEvents)
                .Concat(globalEvents)
                .ToList();

            return GenerateCalendar(year, month, combinedEvents);
        }
    }
}
