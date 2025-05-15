namespace Eventflow.ViewModels.Admin.Component {
   public class ManageUsersViewModel {
      public List<AdminUserViewModel> Users { get; set; } = new();
      public string SearchTerm { get; set; } = string.Empty;
      public string SelectedRole { get; set; } = "All";
      public string SelectedStatus { get; set; } = "All";
   }
}