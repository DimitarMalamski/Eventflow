using Eventflow.Services.Interfaces;

namespace Eventflow.Services
{
    public class CalendarService : ICalendarService
    {
        public string GenerateCalendarHtml(int year, int month)
        {
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayOfWeek = ((int)firstDay.DayOfWeek == 0) ? 6 : ((int)firstDay.DayOfWeek - 1);

            var sb = new System.Text.StringBuilder();

            sb.Append("<table class='table table-bordered text-center align-middle'>");
            sb.Append("<thead class='table-primary'><tr>");
            sb.Append("<th>Mon</th><th>Tue</th><th>Wed</th><th>Thu</th><th>Fri</th><th>Sat</th><th>Sun</th>");
            sb.Append("</tr></thead><tbody><tr>");

            int dayCounter = 1;
            for (int i = 0; i < startDayOfWeek; i++)
                sb.Append("<td class='align-top' style='min-height: 100px;'></td>");

            for (int i = startDayOfWeek; i < 7; i++)
            {
                sb.Append($"<td class='align-top p-2' style='min-height: 100px;'><div class='fw-bold'>{dayCounter}</div></td>");
                dayCounter++;
            }
            sb.Append("</tr>");

            while (dayCounter <= daysInMonth)
            {
                sb.Append("<tr>");
                for (int i = 0; i < 7; i++)
                {
                    if (dayCounter <= daysInMonth)
                    {
                        sb.Append($"<td class='align-top p-2' style='min-height: 100px;'>");
                        sb.Append($"<div class='fw-bold'>{dayCounter}</div>");
                        sb.Append($"<div class='text-danger small'>Holiday Title</div>");
                        sb.Append("</td>");
                        dayCounter++;
                    }
                    else
                    {
                        sb.Append("<td class='align-top' style='min-height: 100px;'></td>");
                    }
                }
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            return sb.ToString();
        }
    }
}
