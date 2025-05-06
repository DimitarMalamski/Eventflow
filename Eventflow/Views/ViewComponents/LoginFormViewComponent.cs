using Eventflow.ViewModels.Account.Form;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class LoginFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LoginViewModel model)
        {
            return View(model);
        }
    }
}
