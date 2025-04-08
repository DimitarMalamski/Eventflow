using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CreatePersonalEventSelectViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CreatePersonalEventSelectModel model)
        {
            return View(model);
        }
    }
}
