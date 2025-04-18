using Eventflow.Application.Services;
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
        public async Task<IActionResult> Index(string state = "unread")
        {
            int userId = GetUserId(HttpContext.Session);

            ReminderStatus status = state.ToLower() switch
            {
                "read" => ReminderStatus.Read,
                _ => ReminderStatus.Unread
            };

            var result = await _personalEventReminderService
                .GetPaginatedPersonalRemindersAsync(userId, status, page: 1, pageSize: 5);

            var model = new ReminderPageViewModel
            {
                CurrentStatus = status,
                Reminders = result.PersonalReminders,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
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
        [Route("Reminder/Create")]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create([FromBody] ReminderRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || model.PersonalEventId <= 0 || model.ReminderDate == default)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid reminder data."
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
        public async Task<IActionResult> GetRemindersPartial(string state = "unread", int page = 1)
        {
            int userId = GetUserId(HttpContext.Session);

            ReminderStatus status = state.ToLower() == "read" 
                ? ReminderStatus.Read 
                : ReminderStatus.Unread;

            var paginatedResult = await _personalEventReminderService
                    .GetPaginatedPersonalRemindersAsync(userId, status, page, PageSize);

            var model = new ReminderPageViewModel
            {
                CurrentStatus = status,
                Reminders = paginatedResult.PersonalReminders,
                TotalPages = paginatedResult.TotalPages,
                CurrentPage = paginatedResult.CurrentPage
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
    }
}
