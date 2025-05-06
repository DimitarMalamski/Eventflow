using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CreatePersonalEventFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CreatePersonalEventViewModel model)
        {
            return View(model);
        }
    }
}
