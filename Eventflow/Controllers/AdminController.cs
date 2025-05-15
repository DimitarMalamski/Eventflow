using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.DTOs.DTOs;
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
      [RequireAdmin]
      public async Task<IActionResult> Users(string search = "",
         string role = "All",
         string status = "All") {
            var userDtos = await _userService.GetAllUsersAsync();

            var filtered = userDtos
               .Where(u =>
                     (string.IsNullOrWhiteSpace(search) || u.Username.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                     (role == "All" || u.RoleName == role) &&
                     (status == "All" || u.Status == status))
               .ToList();

            var vm = new ManageUsersViewModel
            {
               Users = filtered.Select(dto => new AdminUserViewModel {
                  Id = dto.Id,
                  Username = dto.Username,
                  Email = dto.Email,
                  RoleName = dto.RoleName,
                  Status = dto.Status
               }).ToList(),
               SearchTerm = search,
               SelectedRole = role,
               SelectedStatus = status
            };

            return View(vm);
      }

      [HttpPost]
      [RequireAdmin]
      public async Task<IActionResult> UpdateUser([FromBody] UserEditDto dto) {
         try {
            bool update = await _userService.UpdateUserAsync(dto);
            return Json(new {
               success = update 
            });
         }
         catch (Exception ex) {
            return Json(new { success = false, message = ex.Message });
         }
      }
   }
}