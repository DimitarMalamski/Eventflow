using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.Category;

namespace Eventflow.Domain.Models.Entities
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
