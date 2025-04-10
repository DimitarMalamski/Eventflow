using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class InviteUserModalViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(int eventId)
        {
            var model = new InviteUserModalViewModel
            {
                EventId = eventId,
            };

            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
