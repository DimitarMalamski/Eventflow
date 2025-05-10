namespace Eventflow.DTOs.DTOs {
   public class PaginatedInvitesDto {
      public List<InviteDto> Invites { get; set; } = new List<InviteDto>();
      public int TotalPages { get; set; }
      public int CurrentPage { get; set; }
   } 
}