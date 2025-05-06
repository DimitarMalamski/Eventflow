using Eventflow.ViewModels.Calendar;
using Eventflow.ViewModels.Calendar.Component;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarRowViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<CalendarDayViewModel> days, int startIndex)
        {
            var modal = new CalendarRowViewModel()
            {
                Days = days,
                StartIndex = startIndex
            };

            return View(modal);
        }
    }
}
