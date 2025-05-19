namespace Eventflow.DTOs.DTOs {
   public class EventParticipantDto {
      public int UserId { get; set; }
      public string Username { get; set; } = "Unknown";
      public string Email { get; set; } = null!;
      public string Status { get; set; } = null!;
   }
}