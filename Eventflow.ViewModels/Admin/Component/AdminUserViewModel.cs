namespace Eventflow.ViewModels.Admin.Component {
   public class AdminUserViewModel {
      public int Id { get; set; }
      public string Username { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string RoleName { get; set; } = "User"; // or "Admin"
      public string Status { get; set; } = "Active"; // or "Banned"
   }
}