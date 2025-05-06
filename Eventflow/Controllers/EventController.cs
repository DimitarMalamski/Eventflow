using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Models.Entities;
using Eventflow.Utilities;
using Eventflow.ViewModels.Calendar;
using Eventflow.ViewModels.Calendar.Component;
using Eventflow.ViewModels.Calendar.Page;
using Eventflow.ViewModels.Category;
using Eventflow.ViewModels.Continent;
using Eventflow.ViewModels.PersonalEvent.Form;
using Eventflow.ViewModels.Shared.Component;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class EventController : Controller
    {
        private readonly ICalendarNavigationService _calendarNavigationService;
        private readonly ICalendarService _calendarService;
        private readonly IContinentService _continentService;
        private readonly ICategoryService _categoryService;
        private readonly IPersonalEventService _personalEventService;
        private readonly IPersonalEventReminderService _personalEventReminderService;
        private readonly IInviteService _inviteService;
        public EventController(ICalendarNavigationService calendarNavigationService,
            ICalendarService calendarService,
            IContinentService continentService,
            ICategoryService categoryService,
            IPersonalEventService personalEventService,
            IPersonalEventReminderService personalEventReminderService,
            IInviteService inviteService
            )
        {
            _calendarNavigationService = calendarNavigationService;
            _calendarService = calendarService;
            _continentService = continentService;
            _categoryService = categoryService;
            _personalEventService = personalEventService;
            _personalEventReminderService = personalEventReminderService;
            _inviteService = inviteService;
        }

        [RequireUserOrAdmin]
        public async Task<IActionResult> MyEvents(int? year, int? month)
        {
            int userId = GetUserId(HttpContext.Session);

            if (GetUserRoleId(HttpContext.Session) == 0)
            {
                TempData["Error"] = "You do not have access to this page.";
                return RedirectToAction("Login", "Account");
            }

            var model = await ComposeMyEventsViewModel(userId, year ?? DateTime.Now.Year, month ?? DateTime.Now.Month);

            return View(model);
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create()
        {
            var model = new CreatePersonalEventViewModel
            {
                Date = DateTime.Now,
                Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [RequireUserOrAdmin]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePersonalEventViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                    })
                    .ToList();

                return View(model);
            }

            int userId = GetUserId(HttpContext.Session);

            var newEvent = new PersonalEvent
            {
                Title = model.Title,
                Description = model.Description,
                Date = model.Date,
                CategoryId = model.CategoryId,
                UserId = userId,
                IsCompleted = false
            };

            await _personalEventService.CreateAsync(newEvent);

            return RedirectToAction("MyEvents");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var personalEvent = await _personalEventService.GetPersonalEventByIdAsync(id);

            if (personalEvent == null)
            {
                return NotFound();
            }

            var model = new CreatePersonalEventViewModel
            {
                Id = personalEvent.Id,
                Title = personalEvent.Title,
                Description = personalEvent.Description,
                Date = personalEvent.Date,
                CategoryId = personalEvent.CategoryId,
                Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                    })
                    .ToList()
            };

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreatePersonalEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = (await _categoryService.GetAllCategoriesAsync())
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                    })
                    .ToList();

                return View("Create", model);
            }

            int userId = GetUserId(HttpContext.Session);

            var existingPersonalEvent = await _personalEventService.GetPersonalEventByIdAsync(model.Id!.Value);

            if (existingPersonalEvent == null || existingPersonalEvent.UserId != userId)
            {
                return Unauthorized();
            }

            existingPersonalEvent.Title = model.Title;
            existingPersonalEvent.Description = model.Description;
            existingPersonalEvent.Date = model.Date;
            existingPersonalEvent.CategoryId = model.CategoryId;

            await _personalEventService.UpdatePersonalEventAsync(existingPersonalEvent);

            return RedirectToAction("MyEvents");
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> LoadPersonalCalendarPartial(int? month, int? year)
        {
            int userId = GetUserId(HttpContext.Session);
            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var ownEvents = await _personalEventService.GetEventsWithCategoryNamesAsync(userId, normalizedYear, normalizedMonth);
            var invitedEvents = await _personalEventService.GetAcceptedInvitedEventsAsync(userId, normalizedYear, normalizedMonth);
            var combinedEvents = ownEvents.Concat(invitedEvents).ToList();

            var calendarDto = _calendarService.GenerateCalendar(normalizedYear, normalizedMonth, combinedEvents);

            var calendarViewModel = new CalendarViewModel
            {
                Year = calendarDto.Year,
                Month = calendarDto.Month,
                Days = calendarDto.Days.Select(day => new CalendarDayViewModel
                {
                    DayNumber = day.DayNumber,
                    IsToday = day.IsToday,
                    Date = day.Date,
                    PersonalEvents = day.PersonalEvents.Select(e => new PersonalEventWithCategoryNameViewModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Date = e.Date,
                        IsCompleted = e.IsCompleted,
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        UserId = e.UserId,
                        IsInvited = e.IsInvited,
                        CreatorUsername = e.CreatorUsername,
                        IsCreator = e.IsCreator,
                        ParticipantUsernames = e.ParticipantUsernames
                    }).ToList(),
                    NationalEvents = day.NationalEvents.Select(n => new NationalEventViewModel
                    {
                        Title = n.Title,
                        Description = n.Description,
                        Date = n.Date,
                        CountryId = n.CountryId,
                        CountryName = n.CountryName
                    }).ToList()
                }).ToList()
            };

            var model = new CalendarComponentViewModel
            {
                Calendar = calendarViewModel,
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                }
            };

            return PartialView("~/Views/Shared/Partials/Calendar/_CalendarWrapper.cshtml", model);
        }
        private async Task<CalendarPageViewModel> ComposeMyEventsViewModel(int userId, int year, int month)
        {
            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var ownEvents = await _personalEventService.GetEventsWithCategoryNamesAsync(userId, normalizedYear, normalizedMonth);
            var invitedEvents = await _personalEventService.GetAcceptedInvitedEventsAsync(userId, normalizedYear, normalizedMonth);
            var combinedEvents = ownEvents.Concat(invitedEvents).ToList();

            var calendarDto = _calendarService.GenerateCalendar(normalizedYear, normalizedMonth, combinedEvents);

            var calendarViewModel = new CalendarViewModel
            {
                Year = calendarDto.Year,
                Month = calendarDto.Month,
                Days = calendarDto.Days.Select(day => new CalendarDayViewModel
                {
                    DayNumber = day.DayNumber,
                    IsToday = day.IsToday,
                    Date = day.Date,
                    PersonalEvents = day.PersonalEvents.Select(e => new PersonalEventWithCategoryNameViewModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Date = e.Date,
                        IsCompleted = e.IsCompleted,
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        UserId = e.UserId,
                        IsInvited = e.IsInvited,
                        CreatorUsername = e.CreatorUsername,
                        IsCreator = e.IsCreator,
                        ParticipantUsernames = e.ParticipantUsernames
                    }).ToList(),
                    NationalEvents = day.NationalEvents.Select(n => new NationalEventViewModel
                    {
                        Title = n.Title,
                        Description = n.Description,
                        Date = n.Date,
                        CountryId = n.CountryId,
                        CountryName = n.CountryName
                    }).ToList()
                }).ToList()
            };

            bool hasUnreadReminders = await _personalEventReminderService.HasUnreadRemindersForTodayAsync(userId);
            bool hasPendingInvites = await _inviteService.HasPendingInvitesAsync(userId);

            return new CalendarPageViewModel
            {
                Continents = (await _continentService.OrderContinentByNameAsync())
                    .Select(c => new ContinentViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList(),
                Calendar = calendarViewModel,
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                },
                SidebarViewModel = new SidebarViewModel
                {
                    Context = "MyEvents",
                    Buttons = SidebarViewModelBuilder.Build(
                        context: "MyEvents",
                        isLoggedIn: true,
                        hasUnreadReminders: hasUnreadReminders,
                        hasPendingInvites: hasPendingInvites
                    )
                }
            };
        }
    }
}
