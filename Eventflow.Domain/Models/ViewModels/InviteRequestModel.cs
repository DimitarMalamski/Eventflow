namespace Eventflow.Domain.Models.ViewModels
{
    public class InviteRequestModel
    {
        public int EventId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
