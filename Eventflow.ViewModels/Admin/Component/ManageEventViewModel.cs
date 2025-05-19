namespace Eventflow.ViewModels.Admin.Component {
   public class ManageEventViewModel {
      public int EventId { get; set; }
      public string Title { get; set; } = "No Title";
      public string Description { get; set; } = "No Description";
      public DateTime Date { get; set; }
      public string? CategoryName { get; set; }
      public string OwnerUsername { get; set; } = "Unknown";
      public List<EventParticipantViewModel> Participants { get; set; } = new List<EventParticipantViewModel>();
   }
}