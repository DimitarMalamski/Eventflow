using Eventflow.Domain.Models.Common;

namespace Eventflow.Domain.Models.ViewModels
{
    public class InvitePageViewModel
    {
        public List<InviteBoxViewModel> Invites { get; set; } = new List<InviteBoxViewModel>();
        public int CurrentStatusId { get; set; }
        public List<DropdownOption> StatusOptions { get; set; } = new List<DropdownOption>();
    }
}
