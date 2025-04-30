using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Common;

namespace Eventflow.Domain.Models.ViewModels
{
    public class InvitePageViewModel
    {
        public List<InviteBoxViewModel> Invites { get; set; } = new List<InviteBoxViewModel>();
        public InviteStatus CurrentStatus { get; set; } = InviteStatus.Pending;
        public List<DropdownOption> StatusOptions { get; set; } = new List<DropdownOption>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }
}
