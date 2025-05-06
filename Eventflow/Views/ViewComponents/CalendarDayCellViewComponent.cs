using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarDayCellViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CalendarDay day, int dayIndex)
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
