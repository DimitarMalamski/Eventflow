using System.ComponentModel.DataAnnotations;

namespace Eventflow.Models.Models
{
    public class SharedEvent
    {
        [Required]
        public int PersonalEventId { get; set; }

        [Required]
        public int InvitedUserId { get; set; }

        [Required]
        public int StatusId { get; set; }
    }
}
