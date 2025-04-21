using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Models.ViewModels;
using Eventflow.Utilities;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IPersonalEventReminderService _personalEventReminderService;
        private readonly IInviteService _inviteService;
        public MessagesController(
            IPersonalEventReminderService personalEventReminderService,
            IInviteService inviteService)
        {
            _inviteService = inviteService;
            _personalEventReminderService = personalEventReminderService;
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> HasNewMessages()
        {
            var userId = GetUserId(HttpContext.Session);

            bool hasUnreadReminders = await _personalEventReminderService.HasUnreadRemindersForTodayAsync(userId);
            bool hasPendingInvites = await _inviteService.HasPendingInvitesAsync(userId);

            return Json(new
            {
                hasNotifications = hasUnreadReminders || hasPendingInvites,
                hasUnreadReminders,
                hasPendingInvites
            });
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId(HttpContext.Session);

            var sidebarViewModel = new SidebarViewModel
            {
                Context = "Messages",
                Buttons = SidebarViewModelBuilder.Build(
                    context: "Messages",
                    isLoggedIn: true
                )
            };

            ViewData["Title"] = "My messages";

            return View(sidebarViewModel);
        }
    }
}
