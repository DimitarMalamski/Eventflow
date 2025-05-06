using Eventflow.Domain.Models.Entities;
using Eventflow.Utilities;
using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Views.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string context, List<Continent>? continents =  null, int? selectedCountryId = null)
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
                Buttons = buttons,
                SelectedCountryId = selectedCountryId ?? 0
            };

            return View("Default", viewModel);
        }
    }
}
