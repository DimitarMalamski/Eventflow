using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.Category;

namespace Eventflow.Domain.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(categoryNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
