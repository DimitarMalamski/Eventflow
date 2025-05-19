using Eventflow.ViewModels.Category;

namespace Eventflow.ViewModels.Admin.Component {
   public class ManageEventsFilterViewModel {
      public string? SearchTerm { get; set; }
      public int? CategoryId { get; set; }
      public string? OwnerUsername { get; set; }
      public DateTime? Date { get; set; }
      public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
   }
}