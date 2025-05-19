namespace Eventflow.ViewModels.Admin.Component {
   public class ManageEventsResultViewModel {
      public ManageEventsFilterViewModel Filters { get; set; } = new();
      public List<ManageEventViewModel> Events { get; set; } = new();
   }
}