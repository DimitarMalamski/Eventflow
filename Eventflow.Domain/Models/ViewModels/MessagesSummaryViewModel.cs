namespace Eventflow.Domain.Models.ViewModels
{
    public class MessagesSummaryViewModel
    {
        public int PendingInvitesCount { get; set; }
        public int UnreadRemindersCount { get; set; }
        public int LikedRemindersCount { get; set; }
    }
}
