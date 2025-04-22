using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using Eventflow.Utilities;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Views.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string context, List<Continent>? continents =  null)
        {
            var userId = GetUserId(HttpContext.Session);
            var isLoggedIn = IsLoggedIn(HttpContext.Session);

            var buttons = SidebarViewModelBuilder.Build(
                context,
                isLoggedIn
            );

            var viewModel = new SidebarViewModel
            {
                Context = context,
                Continents = continents ?? new List<Continent>(),
                Username = GetUsername(HttpContext.Session),
                IsLoggedin = isLoggedIn,
                Buttons = buttons
            };

            return View("Default", viewModel);
        }
    }
}
