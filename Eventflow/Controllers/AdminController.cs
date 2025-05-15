using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.ViewModels.Admin;
using Eventflow.ViewModels.Admin.Component;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Controllers {
   [RequireAdmin]
   public class AdminController : Controller {
      private readonly IUserService _userService;
      private const int recentUsersCount = 5;
      private const int recentPersonalEventsCount = 5;
      private readonly IPersonalEventService _personalEventService;
      public AdminController(
         IUserService userService,
         IPersonalEventService personalEventService)
      {
         _userService = userService;
         _personalEventService = personalEventService;
      }
      public async Task<IActionResult> Index() {
         var userDtos = await _userService.GetRecentUsersAsync(recentUsersCount);
         var personalEventDtos = await _personalEventService.GetRecentPersonalEventsAsync(recentPersonalEventsCount);

         var vm = new AdminDashboardViewModel {
            TotalUsers = await _userService.GetUserCountAsync(),
            TotalEvents = await _personalEventService.GetPersonalEventsCountAsync(),
            RecentUsers = userDtos.Select(u => new RecentUserViewModel {
               Username = u.Username,
               Email = u.Email
            }).ToList(),
            RecentEvents = personalEventDtos.Select(pe => new RecentPersonalEventViewModel {
               Title = pe.Title,
               CreatorUsername = pe.CreatorUsername
            }).ToList()
         };

         return View(vm);
      }
   }
}