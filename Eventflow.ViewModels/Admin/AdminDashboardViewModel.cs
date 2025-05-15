using Eventflow.ViewModels.Admin.Component;

namespace Eventflow.ViewModels.Admin {
   public class AdminDashboardViewModel {
      public int TotalUsers { get; set; }
      public int TotalEvents { get; set; }
      public List<RecentUserViewModel> RecentUsers { get; set; } = new List<RecentUserViewModel>();
      public List<RecentPersonalEventViewModel> RecentEvents { get; set; } = new List<RecentPersonalEventViewModel>();
   }
}