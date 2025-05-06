using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class FormInputViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FormInputModel model)
        {
            return View(model);
        }
    }
}
