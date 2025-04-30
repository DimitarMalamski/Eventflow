using Eventflow.Domain.Enums;

namespace Eventflow.Domain.Models.ViewModels
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? State { get; set; }
    }
}
