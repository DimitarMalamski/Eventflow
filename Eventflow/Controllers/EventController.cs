using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
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
        public EventController(ICalendarNavigationService calendarNavigationService,
            ICalendarService calendarService,
            IContinentService continentService,
            ICategoryService categoryService,
            IPersonalEventService personalEventService
            )
        {
            _calendarNavigationService = calendarNavigationService;
            _calendarService = calendarService;
            _continentService = continentService;
            _categoryService = categoryService;
            _personalEventService = personalEventService;
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

            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var events = await _personalEventService.GetEventsWithCategoryNamesAsync(userId, normalizedYear, normalizedMonth); ;

            var model = new CalendarPageViewModel
            {
                Continents = await _continentService.OrderContinentByNameAsync(),
                Calendar = _calendarService.GenerateCalendar(normalizedYear, normalizedMonth, events),
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                }
            };

            return View(model);
        }

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create()
        {
            var model = new CreatePersonalEventViewModel
            {
                Date = DateTime.Now,
                Categories = await _categoryService.GetAllCategoriesAsync()
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
                model.Categories = await _categoryService.GetAllCategoriesAsync();
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
            var personalEvent = await _personalEventService.GetByIdAsync(id);

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
                Categories = await _categoryService.GetAllCategoriesAsync()
            };

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreatePersonalEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                return View("Create", model);
            }

            int userId = GetUserId(HttpContext.Session);

            var existingPersonalEvent = await _personalEventService.GetByIdAsync(model.Id!.Value);

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
    }
}
