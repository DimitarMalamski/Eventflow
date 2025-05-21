namespace Eventflow.DTOs.DTOs {
   public class InviteAdminDto {
      public int Id { get; set; }
      public int EventId { get; set; }
      public int InvitedUserId { get; set; }
      public int StatusId { get; set; }
      public string Status { get; set; } = string.Empty;
   }
}