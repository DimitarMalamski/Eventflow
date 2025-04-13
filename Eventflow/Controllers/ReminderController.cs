using Eventflow.Application.Services;
using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
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
        public async Task<IActionResult> Index(string filter = "unread")
        {
            int userId = GetUserId(HttpContext.Session);

            List<ReminderBoxViewModel> reminders = new List<ReminderBoxViewModel>();

            bool isRead = filter.ToLower() == "read";

            if (isRead)
            {
                reminders = await _personalEventReminderService.GetRemindersWithEventTitlesByUserIdAsync(userId, isRead: true);
            }
            else
            {
                reminders = await _personalEventReminderService.GetTodaysUnreadRemindersAsync(userId);
            }

            var model = new ReminderPageViewModel
            {
                CurrentFilter = filter,
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
            return RedirectToAction("Index", new { filter = "unread" });
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
                IsRead = false
            };

            await _personalEventReminderService.CreatePersonalEventReminderAsync(reminder);

            return Json(new
            {
                success = true,
                message = "Reminder created successfully!"
            });
        }
    }
}
