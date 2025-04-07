using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CalendarViewModel calendar)
        {
            return View(calendar);
        }
    }
}
