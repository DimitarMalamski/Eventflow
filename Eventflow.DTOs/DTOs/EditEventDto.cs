namespace Eventflow.DTOs.DTOs {
   public class EditEventDto {
      public int EventId { get; set; }
      public string Title { get; set; } = "";
      public string? Description { get; set; }
      public DateTime Date { get; set; }
      public int? CategoryId { get; set; }
   }
}