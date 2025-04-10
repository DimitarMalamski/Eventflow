using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services
{
    public class PersonalEventService : IPersonalEventService
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly ICategoryRepository _categoryRepository;
        public PersonalEventService(IPersonalEventRepository personalEventRepository,
            ICategoryRepository categoryRepository)
        {
            _personalEventRepository = personalEventRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task CreateAsync(PersonalEvent personalEvent)
            => await _personalEventRepository.CreateEventAsync(personalEvent);
        public async Task<PersonalEvent?> GetByIdAsync(int id)
            => await _personalEventRepository.GetByIdAsync(id);
        public async Task<List<PersonalEventWithCategoryNameViewModel>> GetEventsWithCategoryNamesAsync(int userId, int year, int month)
        {
            var personalEvents = await _personalEventRepository.GetByUserAndMonthAsync(userId, year, month);
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            var categoryMap = categories.ToDictionary(c => c.Id, c => c.Name);

            var personalEventsWithCategoryName = personalEvents.Select(pe => new PersonalEventWithCategoryNameViewModel
            {
                Id = pe.Id,
                Title = pe.Title,
                Description = pe.Description,
                Date = pe.Date,
                IsCompleted = pe.IsCompleted,
                CategoryId = pe.CategoryId,
                UserId = userId,
                CategoryName = pe.CategoryId.HasValue && categoryMap.ContainsKey(pe.CategoryId.Value)
                            ? categoryMap[pe.CategoryId.Value]
                            : "Uncategorized"
            }).ToList();

            return personalEventsWithCategoryName;
        }
        public async Task UpdatePersonalEventAsync(PersonalEvent personalEvent)
            => await _personalEventRepository.UpdatePersonalEventAsync(personalEvent);
    }
}
