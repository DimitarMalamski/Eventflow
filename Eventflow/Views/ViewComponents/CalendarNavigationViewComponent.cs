using Eventflow.ViewModels.Calendar;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CalendarNavigationViewModel navigation)
        {
            return View(navigation);
        }
    }
}
