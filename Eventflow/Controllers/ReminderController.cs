using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Models.DTOs;
using Eventflow.Domain.Models.Entities;
using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class ReminderController : Controller
    {
        private readonly IPersonalEventReminderService _personalEventReminderService;
        private readonly IPersonalEventService _personalEventService;
        private readonly IInviteService _inviteService;
        private const int PageSize = 5;
        public ReminderController(IPersonalEventReminderService personalEventReminderService, 
            IPersonalEventService personalEventService,
            IInviteService inviteService)
        {
            _personalEventReminderService = personalEventReminderService;
            _personalEventService = personalEventService;
            _inviteService = inviteService;
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Index(string state = "unread",
            string? search = null,
            string? sortBy = null,
            string? filterBy = null)
        {
            int userId = GetUserId(HttpContext.Session);
            var reminderStatus = ParseStatus(state);

            PaginatedReminderDto dto;

            if (filterBy == "liked")
            {
                dto = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                    userId, search, sortBy, page: 1, PageSize);
            }
            else
            {
                ReminderStatus status = reminderStatus;
                dto = await _personalEventReminderService
                    .GetPaginatedFilteredPersonalRemindersAsync(userId, status, search, sortBy, page: 1, PageSize);
            }

            var vm = new PaginatedRemindersViewModel
            {
                CurrentPage = dto.CurrentPage,
                TotalPages = dto.TotalPages,
                PersonalReminders = dto.PersonalReminders.Select(r => new ReminderBoxViewModel
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    Title = r.Title,
                    Description = r.Description,
                    Date = r.Date,
                    Status = r.Status,
                    EventTitle = r.EventTitle ?? "Unknown",
                    IsLiked = r.IsLiked
                }).ToList()
            };

            var model = new ReminderPageViewModel
            {
                CurrentStatus = reminderStatus,
                Reminders = vm.PersonalReminders,
                TotalPages = vm.TotalPages,
                CurrentPage = vm.CurrentPage,
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

            try
            {
                await _personalEventReminderService.MarkPersonalEventReminderAsReadAsync(id, userId);
                return Ok();
            }
            catch (ReminderNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedReminderAccessException)
            {
                return Forbid();
            }
            
        }

        [HttpPost]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create([FromBody] ReminderRequestViewModel model)
        {
            if (model.PersonalEventId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid event ID!"
                });
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return Json(new
                {
                    success = false,
                    message = "Title is required!"
                });
            }

            if (model.ReminderDate == default)
            {
                return Json(new
                {
                    success = false,
                    message = "Reminder date is invalid."
                });
            }

            var personalEvent = await _personalEventService.GetPersonalEventByIdAsync(model.PersonalEventId);
            if (personalEvent == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Event not found"
                });
            }

            int userId = GetUserId(HttpContext.Session);
            bool isOwner = personalEvent.UserId == userId;
            bool isInvited = await _inviteService.HasUserAcceptedInviteAsync(userId, model.PersonalEventId);

            if (!isOwner && !isInvited)
            {
                return Forbid();
            }

            var reminder = new PersonalEventReminder
            {
                Title = model.Title,
                Description = model.Description,
                Date = model.ReminderDate,
                PersonalEventId = model.PersonalEventId,
                Status = ReminderStatus.Unread,
                UserId = userId
            };

            try
            {
                await _personalEventReminderService.CreatePersonalEventReminderAsync(reminder, userId);
            }
            catch (InvalidReminderInputException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedReminderAccessException)
            {
                return Forbid();
            }

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

            try
            {
                ReminderStatus status = ParseStatus(state);

                var dto = await _personalEventReminderService
                        .GetPaginatedFilteredPersonalRemindersAsync(userId, status, search, sortBy, page, PageSize);

                var vm = dto.PersonalReminders.Select(r => new ReminderBoxViewModel
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    Title = r.Title,
                    Description = r.Description,
                    Date = r.Date,
                    Status = r.Status,
                    EventTitle = r.EventTitle ?? "Unknown",
                    IsLiked = r.IsLiked
                })
                .ToList();

                var model = new ReminderPageViewModel
                {
                    CurrentStatus = status,
                    Reminders = vm,
                    TotalPages = dto.TotalPages,
                    CurrentPage = dto.CurrentPage,
                    SearchTerm = search,
                    SortBy = sortBy
                };

                return PartialView("~/Views/Shared/Partials/Reminder/_ReminderListPartial.cshtml", model);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = ex.Message 
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Something went wrong while loading reminders." 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireUserOrAdmin]
        public async Task<IActionResult> ToggleLike(int id)
        {
            int userId = GetUserId(HttpContext.Session);

            try
            {
                bool liked = await _personalEventReminderService.ToggleLikeAsync(id, userId);

                return Json(new
                {
                    success = true,
                    liked
                });
            }
            catch (ReminderNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedReminderAccessException)
            {
                return Forbid();
            }
        }
        private ReminderStatus ParseStatus(string? state)
            => state?.ToLower() == "read" 
                ? ReminderStatus.Read 
                : ReminderStatus.Unread;
    }
}
