using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class DropdownViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DropdownViewModel model)
        {
            return View(model);
        }
    }
}
