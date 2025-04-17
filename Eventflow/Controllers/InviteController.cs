using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
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
        public async Task<IActionResult> Send([FromBody] InviteRequestModel model)
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

            bool alreadyInvited = await _inviteService.InviteExistsAsync(model.EventId, invitedUser.Id);
            if (alreadyInvited)
            {
                return Json(new
                {
                    success = false,
                    message = "This user has already been invited!"
                });
            }

            var invite = new Invite
            {
                PersonalEventId = model.EventId,
                InvitedUserId = invitedUser.Id,
                StatusId = 1,
                CreatedAt = DateTime.Now
            };

            await _inviteService.CreateInviteAsync(invite);

            return Json(new
            {
                success = true,
                message = "Invite sent successfully!"
            });
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Index(int statusId = 1)
        {
            int userId = GetUserId(HttpContext.Session);

            await _inviteService.AutoDeclineExpiredInvitesAsync();

            var invites = await _inviteService.GetInvitesByUserAndStatusAsync(userId, statusId);
            var statuses = await _statusService.GetAllStatusOptionsAsync();

            var model = new InvitePageViewModel
            {
                CurrentStatusId = statusId,
                StatusOptions = statuses,
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
    }
}
