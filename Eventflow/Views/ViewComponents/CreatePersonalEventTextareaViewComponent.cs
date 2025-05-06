using Eventflow.ViewModels.PersonalEvent.Component;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CreatePersonalEventTextareaViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CreatePersonalEventTextareaModel model)
        {
            return View(model);
        }
    }
}
