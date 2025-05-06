using Eventflow.ViewModels.Invite.Component;
using Eventflow.ViewModels.Invite.Enums;
using Eventflow.ViewModels.Shared;

namespace Eventflow.ViewModels.Invite.Page
{
    public class InvitePageViewModel
    {
        public List<InviteBoxViewModel> Invites { get; set; } = new List<InviteBoxViewModel>();
        public InviteStatusEnum CurrentStatus { get; set; } = InviteStatusEnum.Pending;
        public List<DropdownOptionViewModel> StatusOptions { get; set; } = new List<DropdownOptionViewModel>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }
}
