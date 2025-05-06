using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Helper;
using Eventflow.Domain.Models.Entities;
using Eventflow.ViewModels.Invite.Component;
using Eventflow.ViewModels.Invite.Enums;
using Eventflow.ViewModels.Invite.Page;
using Eventflow.ViewModels.Invite.Request;
using Eventflow.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class InviteController : Controller
    {
        private readonly IUserService _userService;
        private readonly IInviteService _inviteService;
        private readonly IStatusService _statusService;
        public InviteController(IUserService userService,
            IInviteService inviteService,
            IStatusService statusService)
        {
            _inviteService = inviteService;
            _userService = userService;
            _statusService = statusService;
        }

        [HttpPost]
        [Route("Invite/Send")]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Send([FromBody] InviteRequestViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || model.EventId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid invite data."
                });
            }

            var invitedUser = await _userService.GetUserByUsernameAsync(model.Username);

            if (invitedUser == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User does not exist!"
                });
            }

            int currentUserId = GetUserId(HttpContext.Session);

            if (invitedUser.Id == currentUserId)
            {
                return Json(new
                {
                    success = false,
                    message = "You cannot invite yourself!"
                });
            }

            var invite = new Invite
            {
                PersonalEventId = model.EventId,
                InvitedUserId = invitedUser.Id,
                StatusId = InviteStatusHelper.Pending,
                CreatedAt = DateTime.Now
            };

            var result = await _inviteService.CreateOrResetInviteAsync(invite);

            return result switch
            {
                InviteActionResult.Created
                    => Json(new
                    {
                        success = true,
                        message = "Invite sent successfully!"
                    }),
                InviteActionResult.UpdatedToPending
                    => Json(new
                    {
                        success = true,
                        message = "Invite resent successfully!"
                    }),
                InviteActionResult.AlreadyPending
                    => Json(new
                    {
                        success = false,
                        message = "This user already has a unanswered invite!"
                    }),
                InviteActionResult.AlreadyAccepted
                    => Json(new
                    {
                        success = false,
                        message = "User is already part of the event."
                    }),
                _
                    => Json(new
                    {
                        success = false,
                        message = "An unexpected error occurred."
                    })
            };
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Index(int statusId = 0)
        {
            int userId = GetUserId(HttpContext.Session);

            await _inviteService.AutoDeclineExpiredInvitesAsync();

            var invites = await _inviteService.GetInvitesByUserAndStatusAsync(userId, statusId);
            var statuses = await _statusService.GetAllStatusOptionsAsync();

            var statusOptionsViewModel = statuses.Select(s => new DropdownOptionViewModel
            {
                Id = s.Id,
                Name = s.Name,
            })
            .ToList();

            var model = new InvitePageViewModel
            {
                CurrentStatus = (InviteStatusEnum)statusId,
                StatusOptions = statusOptionsViewModel,
                Invites = invites.Select(invite => new InviteBoxViewModel
                {
                    InviteId = invite.Id,
                    EventTitle = invite.PersonalEvent?.Title ?? "[No Title]",
                    InvitedByUsername = invite.PersonalEvent?.User?.Username ?? "[Unknown User]",
                    EventDescription = invite.PersonalEvent?.Description ?? "No description.",
                    EventDate = invite.PersonalEvent?.Date ?? DateTime.MinValue,
                    StatusId = invite.StatusId,
                    CreatedAt = invite.CreatedAt
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int inviteId, int statusId)
        {
            if (inviteId <= 0 || (statusId != 2 && statusId != 3))
            {
                TempData["Error"] = "Invalid response.";
                return RedirectToAction("Index", new { statusId = 1 });
            }

            await _inviteService.UpdateInviteStatusAsync(inviteId, statusId);

            TempData["Success"] = statusId == 2 ? "Invite accepted!" : "Invite declined.";
            return RedirectToAction("Index", new { statusId = 1 });
        }

        [HttpPost]
        [RequireUserOrAdmin]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveEvent(int eventId)
        {
            var userId = GetUserId(HttpContext.Session);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _inviteService.LeaveEventAsync(userId, eventId);

            TempData["Message"] = "You’ve successfully left the event.";
            return RedirectToAction("MyEvents", "Event");
        }
    }
}
