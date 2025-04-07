using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class EventDropdownViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int index, DateTime date, List<PersonalEvent> personalEvents)
        {
            var model = new EventDropdownViewModel
            {
                Index = index,
                Date = date,
                PersonalEvents = personalEvents
            };

            return View(model);
        }
    }
}
