using Eventflow.Domain.Models.Entities;
using Eventflow.Utilities;
using Eventflow.ViewModels;
using Eventflow.ViewModels.Continent;
using Eventflow.ViewModels.Shared.Component;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Views.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string context,
            List<ContinentViewModel>? continents =  null, 
            int? selectedCountryId = null,
            bool hasUnreadReminders = false,
            bool hasPendingInvites = false)
        {
            var userId = GetUserId(HttpContext.Session);
            var isLoggedIn = IsLoggedIn(HttpContext.Session);

            var buttons = SidebarViewModelBuilder.Build(
                context,
                isLoggedIn,
                hasUnreadReminders,
                hasPendingInvites
            );

            var viewModel = new SidebarViewModel
            {
                Context = context,
                Continents = continents ?? new List<ContinentViewModel>(),
                Username = GetUsername(HttpContext.Session),
                IsLoggedin = isLoggedIn,
                Buttons = buttons,
                SelectedCountryId = selectedCountryId ?? 0
            };

            return View("Default", viewModel);
        }
    }
}
