using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class ReminderController : Controller
    {
        private readonly IPersonalEventReminderService _personalEventReminderService;
        private const int PageSize = 5;
        public ReminderController(IPersonalEventReminderService personalEventReminderService)
        {
            _personalEventReminderService = personalEventReminderService;
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Index(string state = "unread",
            string? search = null,
            string? sortBy = null)
        {
            int userId = GetUserId(HttpContext.Session);

            ReminderStatus status = ParseStatus(state);

            var result = await _personalEventReminderService
                .GetPaginatedFilteredPersonalRemindersAsync(userId, status, search, sortBy, page: 1, PageSize);

            var model = new ReminderPageViewModel
            {
                CurrentStatus = status,
                Reminders = result.PersonalReminders,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                SearchTerm = search,
                SortBy = sortBy
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUserOrAdmin]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            int userId = GetUserId(HttpContext.Session);

            var success = await _personalEventReminderService.MarkPersonalEventReminderAsReadAsync(id, userId);

            if (!success)
            {
                return Forbid();
            }
            return Ok();
        }

        [HttpPost]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create([FromBody] ReminderRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return Json(new
                {
                    Success = false,
                    message = "Title is required!"
                });
            }

            if (model.PersonalEventId <= 0)
            {
                return Json(new
                {
                    Success = false,
                    message = "Invalid event ID!"
                });
            }

            if (model.ReminderDate == default)
            {
                return Json(new
                {
                    Success = false,
                    message = "Reminder date is invalid."
                });
            }

            int userId = GetUserId(HttpContext.Session);

            var reminder = new PersonalEventReminder
            {
                Title = model.Title,
                Description = model.Description,
                Date = model.ReminderDate,
                PersonalEventId = model.PersonalEventId,
                Status = ReminderStatus.Unread
            };

            await _personalEventReminderService.CreatePersonalEventReminderAsync(reminder);

            return Json(new
            {
                success = true,
                message = "Reminder created successfully!"
            });
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> GetRemindersPartial(string state = "unread", 
            int page = 1,
            string? search = null,
            string? sortBy = null)
        {
            int userId = GetUserId(HttpContext.Session);

            ReminderStatus status = ParseStatus(state);

            var paginatedResult = await _personalEventReminderService
                    .GetPaginatedFilteredPersonalRemindersAsync(userId, status, search, sortBy, page, PageSize);

            var model = new ReminderPageViewModel
            {
                CurrentStatus = status,
                Reminders = paginatedResult.PersonalReminders,
                TotalPages = paginatedResult.TotalPages,
                CurrentPage = paginatedResult.CurrentPage,
                SearchTerm = search,
                SortBy = sortBy
            };

            return PartialView("~/Views/Shared/Partials/Reminder/_ReminderListPartial.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUserOrAdmin]
        public async Task<IActionResult> ToggleLike(int id)
        {
            int userId = GetUserId(HttpContext.Session);

            var result = await _personalEventReminderService.ToggleLikeAsync(id, userId);

            if (result == null)
            {
                return Forbid();
            }

            return Json(new
            {
                success = true,
                liked = result.Value
            });
        }
        private ReminderStatus ParseStatus(string? state)
            => state?.ToLower() == "read" 
                ? ReminderStatus.Read 
                : ReminderStatus.Unread;
    }
}
