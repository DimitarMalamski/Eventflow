using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Views.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string context, List<Continent>? continents =  null)
        {
            var viewModel = new SidebarViewModel
            {
                Context = context,
                Continents = continents ?? new List<Continent>(),
                Username = GetUsername(HttpContext.Session),
                IsLoggedin = IsLoggedIn(HttpContext.Session)
            };

            return View("Default", viewModel);
        }
    }
}
