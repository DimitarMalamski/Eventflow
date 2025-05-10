namespace Eventflow.DTOs.DTOs {
   public class InviteDto {
      public int Id { get; set; }
      public string EventTitle { get; set; } = string.Empty;
      public string InvitedByUsername { get; set; } = string.Empty;
      public string? EventDescription { get; set; }
      public DateTime EventDate { get; set; }
      public DateTime CreatedAt { get; set; }
      public int StatusId { get; set; } 
   }
}