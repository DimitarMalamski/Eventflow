using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Eventflow.Common.ValidationConstants.Category;

namespace Eventflow.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(categoryNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
