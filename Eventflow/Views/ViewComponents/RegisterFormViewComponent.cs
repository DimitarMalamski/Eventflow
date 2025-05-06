using Eventflow.ViewModels.Account.Form;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class RegisterFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(RegisterViewModel model)
        {
            return View(model);
        }
    }
}
