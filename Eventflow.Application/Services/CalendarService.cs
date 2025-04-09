using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services
{
    public class CalendarService : ICalendarService
    {
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
        public CalendarViewModel GenerateCalendar(int year, int month)
        {
            return GenerateCalendar(year, month, new List<PersonalEventWithCategoryNameViewModel>());
        }
    }
}
