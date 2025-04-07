using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            return View(daysOfWeek);
        }
    }
}
