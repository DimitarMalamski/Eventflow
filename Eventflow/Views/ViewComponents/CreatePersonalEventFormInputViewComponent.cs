using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Views.ViewComponents
{
    public class CreatePersonalEventFormInputViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CreatePersonalEventFormInputModel model)
        {
            return View(model);
        }
    }
}
