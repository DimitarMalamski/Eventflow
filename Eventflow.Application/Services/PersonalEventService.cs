using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Helper;
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
        private readonly IInviteRepository _inviteRepository;
        public PersonalEventService(IPersonalEventRepository personalEventRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository,
            IInviteRepository inviteRepository)
        {
            _personalEventRepository = personalEventRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _inviteRepository = inviteRepository;
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

                if (creator == null || creator.IsDeleted) {
                    continue;
                }

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
                    IsGlobal = e.IsGlobal,
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
                var creator = await _userRepository.GetUserByIdAsync(pe.UserId);

                if (creator == null || creator.IsDeleted) {
                    continue;
                }

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
                    IsGlobal = pe.IsGlobal,
                    ParticipantUsernames = perticipantsUsernames
                });
            }

            return personalEventsWithCategoryName;
        }
        public async Task UpdatePersonalEventAsync(PersonalEvent personalEvent)
            => await _personalEventRepository.UpdatePersonalEventAsync(personalEvent);
        public async Task<int> GetPersonalEventsCountAsync()
            => await _personalEventRepository.GetPersonalEventsCountAsync();
        public async Task<List<RecentPersonalEventDto>> GetRecentPersonalEventsAsync(int count)
        {
            var personalEvents = await _personalEventRepository.GetRecentPersonalEventsAsync(count);

            var userIds = personalEvents.Select(e => e.UserId).Distinct().ToList();
            var userMap = await _userRepository.GetUsernamesByIdsAsync(userIds);

            return personalEvents.Select(pe => new RecentPersonalEventDto {
                Title = pe.Title,
                CreatorUsername = userMap.TryGetValue(pe.UserId, out var username) ? username : "Unknown"
            })
            .ToList();
        }
        public async Task<List<ManageEventDto>> GetAllManageEventsAsync()
        {
            var events = await _personalEventRepository.GetAllPersonalEventsAsync();
            var users = await _userRepository.GetAllUsersAsync();
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            var userDict = users.ToDictionary(u => u.Id);
            var categoryDict = categories.ToDictionary(c => c.Id);

            var result = new List<ManageEventDto>();

            foreach (var ev in events) {
                var invites = await _inviteRepository.GetInvitesByEventIdAsync(ev.Id);

                var participantDtos = invites
                    .Where(inv => inv.StatusId != InviteStatusHelper.Declined)
                    .Select(inv => {
                    var user = userDict.GetValueOrDefault(inv.InvitedUserId);
                    return new EventParticipantDto {
                        UserId = inv.InvitedUserId,
                        Username = user?.Username ?? "Unknown",
                        Email = user?.Email ?? "Unknown",
                        Status = Enum.GetName(typeof(InviteStatus), inv.StatusId) ?? "Unknown"
                    };
                }).ToList();

                result.Add(new ManageEventDto {
                    EventId = ev.Id,
                    Title = ev.Title,
                    Description = ev.Description ?? "No Description",
                    Date = ev.Date,
                    CategoryId = ev.CategoryId,
                    CategoryName = ev.CategoryId.HasValue && categoryDict.ContainsKey(ev.CategoryId.Value)
                        ? categoryDict[ev.CategoryId.Value].Name
                        : null,
                    OwnerUsername = userDict.ContainsKey(ev.UserId)
                        ? userDict[ev.UserId].Username
                        : "Unknown",
                    Participants = participantDtos
                });
            }

            return result;
        }
        public async Task<ManageEventDto?> UpdateEventFromAdminAsync(EditEventDto dto)
        {
           var personalEvent = await _personalEventRepository.GetPersonalEventByIdAsync(dto.EventId);

           if (personalEvent == null) {
                return null;
           }

           personalEvent.Title = dto.Title;
           personalEvent.Description = dto.Description;
           personalEvent.Date = dto.Date;
           personalEvent.CategoryId = dto.CategoryId;

           await _personalEventRepository.UpdatePersonalEventAsync(personalEvent);

           var user = await _userRepository.GetUserByIdAsync(personalEvent.UserId);
           var category = dto.CategoryId.HasValue
                ? await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId.Value)
                : null;

           var participantDtos = await GetParticipantsByEventIdAsync(personalEvent.Id);

            return new ManageEventDto
            {
                EventId = personalEvent.Id,
                Title = personalEvent.Title,
                Description = personalEvent.Description ?? "No Description",
                Date = personalEvent.Date,
                CategoryId = personalEvent.CategoryId,
                CategoryName = category?.Name,
                OwnerUsername = user?.Username ?? "Unknown",
                Participants = participantDtos
            };
        }
        public async Task<List<EventParticipantDto>> GetParticipantsByEventIdAsync(int eventId)
        {
            var invites = (await _inviteRepository.GetInvitesByEventIdAsync(eventId))
                        .Where(i => i.StatusId != InviteStatusHelper.KickedOut)
                        .ToList();
            var allUsers = await _userRepository.GetAllUsersAsync();
            var userDict = allUsers.ToDictionary(u => u.Id);

            return invites.Select(inv => {
                var user = userDict.GetValueOrDefault(inv.InvitedUserId);
                return new EventParticipantDto {
                    UserId = inv.InvitedUserId,
                    Username = user?.Username ?? "Unknown",
                    Email = user?.Email ?? "Unknown",
                    Status = Enum.GetName(typeof(InviteStatus), inv.StatusId) ?? "Unknown"
                };
            }).ToList();
        }
        public async Task<bool> SoftDeleteEventAsync(int eventId, int userId)
        {
            var personalEvent = await _personalEventRepository.GetPersonalEventByIdAsync(eventId);

            if (personalEvent == null || personalEvent.IsDeleted) {
                return false;
            }

            if (userId != 0 && personalEvent.UserId != userId) {
                return false;
            }

            await _inviteRepository.DeleteInvitesByEventIdAsync(eventId);

            await _personalEventRepository.SoftDeleteEventAsync(eventId);
            return true;
        }
        public async Task<List<PersonalEventWithCategoryNameDto>> GetGlobalEventsWithCategoryAsync(int year, int month)
        {
            var entities = await _personalEventRepository.GetGlobalEventsAsync(year, month);

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var users = await _userRepository.GetAllUsersAsync();

            return entities.Select(pe => {
                var category =  categories.FirstOrDefault(c => c.Id == pe.CategoryId);
                var user = users.FirstOrDefault(u => u.Id == pe.UserId);

                return new PersonalEventWithCategoryNameDto {
                    Id = pe.Id,
                    Title = pe.Title,
                    Description = pe.Description,
                    Date = pe.Date,
                    CategoryId = pe.CategoryId,
                    CategoryName = category?.Name,
                    UserId = pe.UserId,
                    CreatorUsername = user?.Username,
                    IsCompleted = pe.IsCompleted,
                    IsGlobal = pe.IsGlobal,
                    IsInvited = false,
                    IsCreator = false,
                    ParticipantUsernames = new List<string>()
                };
            }).ToList();
        }
   }
}
