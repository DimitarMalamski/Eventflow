using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;
using Eventflow.DTOs.Enums;

namespace Eventflow.Application.Mapper
{
    public static class ViewModelMapper
    {
        public static class PersonalReminder
        {
            public static ReminderDto ToBoxViewModel(PersonalEventReminder r) => new ReminderDto()
            {
                Id = r.Id,
                EventId = r.PersonalEventId,
                Title = r.Title,
                Description = r.Description,
                Date = r.Date,
                Status = (ReminderStatusEnum)(int)r.Status,
                EventTitle = r.PersonalEvent?.Title ?? "Unknown",
                IsLiked = r.IsLiked,
            };
            public static List<ReminderDto> ToReminderDtoList(IEnumerable<PersonalEventReminder> reminders)
                => reminders?.Select(ToBoxViewModel).ToList()
                    ?? new List<ReminderDto>();
        }

        public static class InviteMapper {
            public static InviteDto ToBoxViewModel(Invite invite) => new InviteDto
            {
                Id = invite.Id,
                EventTitle = invite.PersonalEvent?.Title ?? "[No Title]",
                InvitedByUsername = invite.PersonalEvent?.User?.Username ?? "[Unknown User]",
                EventDescription = invite.PersonalEvent?.Description,
                EventDate = invite.PersonalEvent?.Date ?? DateTime.MinValue,
                CreatedAt = invite.CreatedAt,
                StatusId = invite.StatusId
            };
            public static List<InviteDto> ToInviteDtoList(IEnumerable<Invite> invites)
                => invites?.Select(ToBoxViewModel).ToList()
                    ?? new List<InviteDto>();
        }
    }
}
