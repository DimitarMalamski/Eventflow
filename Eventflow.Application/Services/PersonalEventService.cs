using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;

namespace Eventflow.Application.Services
{
    public class PersonalEventService : IPersonalEventService
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        public PersonalEventService(IPersonalEventRepository personalEventRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository)
        {
            _personalEventRepository = personalEventRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }
        public async Task CreateAsync(PersonalEvent personalEvent)
            => await _personalEventRepository.CreateEventAsync(personalEvent);
        public async Task<List<PersonalEventWithCategoryNameDto>> GetAcceptedInvitedEventsAsync(int userId, int year, int month)
        {
            var acceptedEvents = await _personalEventRepository.GetAcceptedInvitedEventsAsync(userId);

            var filteredEvents = acceptedEvents
                .Where(e => e.Date.Year == year && e.Date.Month == month)
                .ToList();

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoryMap = categories.ToDictionary(c => c.Id, c => c.Name);

            var dtoList = new List<PersonalEventWithCategoryNameDto>();

            foreach (var e in filteredEvents)
            {
                var creator = await _userRepository.GetUserByIdAsync(e.UserId);
                var participantUsernames = await _userRepository.GetUsernamesByEventIdAsync(e.Id);

                dtoList.Add(new PersonalEventWithCategoryNameDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    IsCompleted = e.IsCompleted,
                    CategoryId = e.CategoryId,
                    UserId = e.UserId,
                    CategoryName = e.CategoryId.HasValue && categoryMap.ContainsKey(e.CategoryId.Value)
                        ? categoryMap[e.CategoryId.Value]
                        : "Uncategorized",
                    IsInvited = true,
                    CreatorUsername = creator?.Username ?? "Unknown",
                    IsCreator = e.UserId == userId,
                    ParticipantUsernames = participantUsernames
                });
            }

            return dtoList;
        }
        public async Task<PersonalEvent?> GetPersonalEventByIdAsync(int id)
            => await _personalEventRepository.GetPersonalEventByIdAsync(id);
        public async Task<List<PersonalEventWithCategoryNameDto>> GetEventsWithCategoryNamesAsync(int userId, int year, int month)
        {
            var personalEvents = (await _personalEventRepository.GetByUserAndMonthAsync(userId, year, month))
                .Where(e => e.Date.Date >= DateTime.Today)
                .ToList();

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoryMap = categories.ToDictionary(c => c.Id, c => c.Name);

            var personalEventsWithCategoryName = new List<PersonalEventWithCategoryNameDto>();

            foreach (var pe in personalEvents)
            {
                var perticipantsUsernames = await _userRepository.GetUsernamesByEventIdAsync(pe.Id);

                personalEventsWithCategoryName.Add(new PersonalEventWithCategoryNameDto
                {
                    Id = pe.Id,
                    Title = pe.Title,
                    Description = pe.Description,
                    Date = pe.Date,
                    IsCompleted = pe.IsCompleted,
                    CategoryId = pe.CategoryId,
                    UserId = pe.UserId,
                    CategoryName = pe.CategoryId.HasValue && categoryMap.ContainsKey(pe.CategoryId.Value)
                        ? categoryMap[pe.CategoryId.Value]
                        : "Uncategorized",
                    IsInvited = false,
                    IsCreator = pe.UserId == userId,
                    ParticipantUsernames = perticipantsUsernames
                });
            }

            return personalEventsWithCategoryName;
        }
        public async Task UpdatePersonalEventAsync(PersonalEvent personalEvent)
            => await _personalEventRepository.UpdatePersonalEventAsync(personalEvent);
    }
}
