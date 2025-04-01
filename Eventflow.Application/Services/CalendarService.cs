using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services
{
    public class CalendarService : ICalendarService
    {
        public CalendarViewModel GenerateCalendar(int year, int month)
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
                    DayNumber = null
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                bool isToday = day == DateTime.Today.Day &&
                                month == DateTime.Today.Month &&
                                year == DateTime.Today.Year;

                model.Days.Add(new CalendarDay
                {
                    DayNumber = day,
                    IsToday = isToday
                });
            }

            while (model.Days.Count % 7 != 0)
            {
                model.Days.Add(new CalendarDay
                {
                    DayNumber = null
                });
            }

            return model;
        }
    }
}
