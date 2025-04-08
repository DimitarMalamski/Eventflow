using Eventflow.Domain.Models.ViewModels;
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
