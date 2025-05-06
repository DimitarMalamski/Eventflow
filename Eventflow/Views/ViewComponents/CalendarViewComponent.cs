using Eventflow.ViewModels.Calendar.Component;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CalendarComponentViewModel model)
        {
            return View(model);
        }
    }
}
