namespace Eventflow.DTOs.DTOs {
   public class ManageEventDto {
      public int EventId { get; set; }
      public string Title { get; set; } = "No Title";
      public string Description { get; set; } = "No Description";
      public DateTime Date { get; set; }
      public string? CategoryName { get; set; }
      public string OwnerUsername { get; set; } = "Unknown";
      public List<EventParticipantDto> Participants { get; set; } = new List<EventParticipantDto>();
   }
}