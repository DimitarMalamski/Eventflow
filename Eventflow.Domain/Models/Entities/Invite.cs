using System.ComponentModel.DataAnnotations;

namespace Eventflow.Domain.Models.Entities
{
    public class Invite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PersonalEventId { get; set; }

        [Required]
        public int InvitedUserId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public PersonalEvent? PersonalEvent { get; set; }
    }
}
