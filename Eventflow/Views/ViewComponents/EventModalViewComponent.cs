using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class EventModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
