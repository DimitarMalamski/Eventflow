using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CalendarRowViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<CalendarDay> days, int startIndex)
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
