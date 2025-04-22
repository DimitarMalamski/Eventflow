namespace Eventflow.Domain.Models.ViewModels
{
    public class PersonalEventWithCategoryNameViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = "Uncategorized";
        public int UserId { get; set; }
        public bool IsInvited { get; set; } = false;
        public string CreatorUsername { get; set; } = string.Empty;
        public bool IsCreator { get; set; }
    }
}
