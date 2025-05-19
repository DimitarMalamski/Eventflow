using System.Runtime.CompilerServices;
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
            var filteredDtos = await _userService.GetFilteredAdminUsersAsync(search, role, status);

            var vm = new ManageUsersViewModel
            {
               Users = filteredDtos.Select(dto => new AdminUserViewModel {
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

      [RequireAdmin]
      public async Task<IActionResult> Events() {
         var dtos = await _personalEventService.GetAllManageEventsAsync();

         var vm = dtos.Select(dto => new ManageEventViewModel {
            EventId = dto.EventId,
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date,
            CategoryName = dto.CategoryName,
            OwnerUsername = dto.OwnerUsername,
            Participants = dto.Participants.Select(p => new EventParticipantViewModel
            {
                  Username = p.Username,
                  Email = p.Email,
                  Status = p.Status
            }).ToList()
         }).ToList();

         var resultVm = new ManageEventsResultViewModel
         {
            Events = vm,
            Filters = new ManageEventsFilterViewModel() // empty for now, will support filters later
         };

         return View(resultVm);
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

      [HttpPost]
      [RequireAdmin]
      public async Task<IActionResult> ToggleBan([FromBody] IdDto dto) {

         int id = dto.Id;

         if (id <= 0)
         {
            return BadRequest(new 
            {
               success = false,
               message = "Invalid user ID."
            });
         }

         var user = await _userService.GetUserByIdAsync(id);

         if (user == null)
         {
            return Json(new 
            {
               success = false,
               message = "User not found."
            });
         }

         bool newIsBanned = !user.IsBanned;
         await _userService.UpdateUserBanStatusAsync(id, newIsBanned);

         string newStatus = newIsBanned ? "Banned" : "Active";

         return Json(new 
         {
            success = true,
            newStatus 
         });
      }

      [HttpPost]
      [RequireAdmin]
      public async Task<IActionResult> DeleteUser(int id) {
         bool deleted = await _userService.SoftDeleteUserAsync(id);
         return deleted ? Ok() : BadRequest("Could not delete user!");
      }

      [HttpGet]
      [RequireAdmin]
      public async Task<IActionResult> GetFilteredUsersPartial(string search = "", string role = "All", string status = "All") {
         var filtered = await _userService.GetFilteredAdminUsersAsync(search, role, status);

         var vm = new ManageUsersViewModel
         {
            Users = filtered.Select(dto => new AdminUserViewModel
            {
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

         return PartialView("~/Views/Shared/Partials/Admin/User/_UserTablePartial.cshtml", vm);
      }
   }
}