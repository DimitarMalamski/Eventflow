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

            List<ReminderBoxViewModel> reminders = status switch
            {
                ReminderStatus.Read => await _personalEventReminderService
                    .GetRemindersWithEventTitlesByUserIdAsync(userId, ReminderStatus.Read),

                ReminderStatus.Unread => await _personalEventReminderService
                    .GetTodaysUnreadRemindersAsync(userId),

                _ => new List<ReminderBoxViewModel>()
            };

            var model = new ReminderPageViewModel
            {
                CurrentStatus = status,
                Reminders = reminders
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUserOrAdmin]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _personalEventReminderService.MarkPersonalEventReminderAsReadAsync(id);
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
        public async Task<IActionResult> GetRemindersPartial(string state = "unread")
        {
            int userId = GetUserId(HttpContext.Session);

            ReminderStatus status = state.ToLower() == "read" ? ReminderStatus.Read : ReminderStatus.Unread;

            var reminders = status == ReminderStatus.Read
                ? await _personalEventReminderService.GetRemindersWithEventTitlesByUserIdAsync(userId, ReminderStatus.Read)
                : await _personalEventReminderService.GetTodaysUnreadRemindersAsync(userId);

            return PartialView("~/Views/Shared/Partials/Reminder/_ReminderListPartial.cshtml", reminders);
        }
    }
}
