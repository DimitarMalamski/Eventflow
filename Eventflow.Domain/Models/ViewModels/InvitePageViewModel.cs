using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Models.ViewModels
{
    public class InvitePageViewModel
    {
        public List<InviteBoxViewModel> Invites { get; set; } = new List<InviteBoxViewModel>();
        public int CurrentStatusId { get; set; }
    }
}
