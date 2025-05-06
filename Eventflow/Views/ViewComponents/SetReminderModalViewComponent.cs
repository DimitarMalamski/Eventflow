using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class SetReminderModalViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(int eventId)
        {
            var model = new SetReminderModalViewModel
            {
                PersonalEventId = eventId,
                ReminderDate = DateTime.Now.Date
            };

            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
