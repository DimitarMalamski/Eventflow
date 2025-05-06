using Eventflow.ViewModels.Calendar;
using Eventflow.ViewModels.Calendar.Component;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarDayCellViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CalendarDayViewModel day, int dayIndex)
        {
            var model = new CalendarDayCellViewModel
            {
                Day = day,
                Index = dayIndex
            };

            return View(model);
        }
    }
}
