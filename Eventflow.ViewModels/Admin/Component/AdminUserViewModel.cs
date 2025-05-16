namespace Eventflow.ViewModels.Admin.Component {
   public class AdminUserViewModel {
      public int Id { get; set; }
      public string Username { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string RoleName { get; set; } = "User";
      public string Status { get; set; } = "Active";
   }
}