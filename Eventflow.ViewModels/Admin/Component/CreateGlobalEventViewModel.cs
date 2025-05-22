using System.ComponentModel.DataAnnotations;
using Eventflow.ViewModels.Category;

namespace Eventflow.ViewModels.Admin.Component {
   public class CreateGlobalEventViewModel {
      [Required]
      public string Title { get; set; }

      public string? Description { get; set; }

      [Required]
      [DataType(DataType.Date)]
      public DateTime Date { get; set; }

      public int? CategoryId { get; set; }

      public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
   }
}