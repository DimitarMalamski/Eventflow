using System.ComponentModel.DataAnnotations;

namespace Eventflow.Domain.Models.Entities
{
    public class Bookmarked
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
