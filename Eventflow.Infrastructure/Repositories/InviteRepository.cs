using Eventflow.Domain.Enums;
using Eventflow.Domain.Helper;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly IDbHelper _dbHelper;
        public InviteRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task CreateInviteAsync(Invite invite)
        {
            string createInviteQuery = @"
                    INSERT INTO Invite (PersonalEventId, InvitedUserId, StatusId, CreatedAt)
                    VALUES (@PersonalEventId, @InvitedUserId, @StatusId, @CreatedAt)";

            var parameters = new Dictionary<string, object>()
            {
                { "@PersonalEventId", invite.PersonalEventId },
                { "@InvitedUserId", invite.InvitedUserId },
                { "@StatusId", invite.StatusId },
                { "@CreatedAt", invite.CreatedAt }
            };

            await _dbHelper.ExecuteNonQueryAsync(createInviteQuery, parameters);
        }
        public async Task<InviteActionResult> CreateOrResetInviteAsync(Invite invite)
        {
            var existing = await GetInviteStatusAndIdAsync(invite.PersonalEventId, invite.InvitedUserId);

            if (existing == null)
            {
                await InsertInviteAsync(invite);
                return InviteActionResult.Created;
            }

            var (inviteId, statusId) = existing.Value;

            if (statusId == InviteStatusHelper.Pending)
            {
                return InviteActionResult.AlreadyPending;
            }

            if (statusId == InviteStatusHelper.Accepted)
            {
                return InviteActionResult.AlreadyAccepted;
            }

            await ResetInviteToPendingAsync(inviteId);
            return InviteActionResult.UpdatedToPending;
        }
        public async Task<List<Invite>> GetAllInvitesByUserIdAsync(int userId)
        {
            string getAllInvitedByUserQuery = "SELECT * FROM Invite WHERE InvitedUserId = @UserId";

            var parameters = new Dictionary<string, object>()
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getAllInvitedByUserQuery, parameters);

            List<Invite> invites = new List<Invite>();

            foreach (DataRow row in dt.Rows)
            {
                invites.Add(new Invite
                {
                    Id = Convert.ToInt32(row["Id"]),
                    PersonalEventId = Convert.ToInt32(row["PersonalEventId"]),
                    InvitedUserId = Convert.ToInt32(row["InvitedUserId"]),
                    StatusId = Convert.ToInt32(row["StatusId"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                });
            }

            return invites;
        }
        public async Task<List<Invite>> GetExpiredPendingInvitesAsync()
        {
            string getExpiredPendingInvitesQuery = @"
                    SELECT i.*
                    FROM Invite i
                    INNER JOIN PersonalEvent e ON i.PersonalEventId = e.Id
                    WHERE i.StatusId = @PendingStatus
                    AND CONVERT(date, e.Date) < CONVERT(date, GETDATE())";

            var parameters = new Dictionary<string, object>()
            {
                { "@PendingStatus", InviteStatusHelper.Pending }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getExpiredPendingInvitesQuery, parameters);

            List<Invite> invites = new List<Invite>();

            foreach (DataRow row in dt.Rows)
            {
                invites.Add(new Invite
                {
                    Id = Convert.ToInt32(row["Id"]),
                    PersonalEventId = Convert.ToInt32(row["PersonalEventId"]),
                    InvitedUserId = Convert.ToInt32(row["InvitedUserId"]),
                    StatusId = Convert.ToInt32(row["StatusId"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                });
            }

            return invites;
        }
        public async Task<List<Invite>> GetInvitesByUserAndStatusAsync(int userId, int statusId)
        {
            string getInvitesByUserAndStatusQuery = @"
                                        SELECT 
                                            i.Id AS InviteId, 
                                            i.PersonalEventId, 
                                            i.InvitedUserId, 
                                            i.StatusId, 
                                            i.CreatedAt,

                                            e.Id AS EventId,
                                            e.Title AS EventTitle,
                                            e.Description AS EventDescription,
                                            e.Date AS EventDate,
                                            e.CategoryId,
                                            e.UserId AS CreatorUserId,

                                            u.Id AS UserId,
                                            u.Username AS CreatorUsername
                                        FROM Invite i
                                        INNER JOIN PersonalEvent e ON i.PersonalEventId = e.Id
                                        INNER JOIN [User] u ON e.UserId = u.Id
                                        WHERE i.InvitedUserId = @UserId AND i.StatusId = @StatusId";

            var parameters = new Dictionary<string, object>()
            {
                { "@UserId", userId },
                { "@StatusId", statusId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getInvitesByUserAndStatusQuery, parameters);

            var invites = new List<Invite>();

            foreach (DataRow row in dt.Rows)
            {
                var personalEvent = new PersonalEvent
                {
                    Id = Convert.ToInt32(row["EventId"]),
                    Title = row["EventTitle"].ToString()!,
                    Description = row["EventDescription"]?.ToString(),
                    Date = Convert.ToDateTime(row["EventDate"]),
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null,
                    UserId = Convert.ToInt32(row["CreatorUserId"]),
                    User = new User
                    {
                        Id = Convert.ToInt32(row["UserId"]),
                        Username = row["CreatorUsername"].ToString()!
                    }
                };

                invites.Add(new Invite
                {
                    Id = Convert.ToInt32(row["InviteId"]),
                    PersonalEventId = Convert.ToInt32(row["PersonalEventId"]),
                    InvitedUserId = Convert.ToInt32(row["InvitedUserId"]),
                    StatusId = Convert.ToInt32(row["StatusId"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    PersonalEvent = personalEvent
                });
            }

            return invites;
        }
        public async Task<bool> HasPendingInvitesAsync(int userId)
        {
            string hasUnreadInvitesQuery = @"
                SELECT COUNT(*) 
                FROM Invite i
                WHERE i.InvitedUserId = @UserId 
                  AND i.StatusId = (SELECT Id FROM Status WHERE Name = 'Pending')";

            var parameters = new Dictionary<string, object>()
            {
                { "@UserId", userId }
            };

            var count = (int)await _dbHelper.ExecuteScalarAsync(hasUnreadInvitesQuery, parameters);
            return count > 0;
        }
        public async Task<bool> HasUserAcceptedInviteAsync(int userId, int personalEventId)
        {
            string hasUserAcceptedInviteQuery = @"
                SELECT COUNT(1)
                FROM Invite
                WHERE InvitedUserId = @UserId
                    AND PersonalEventId = @EventId
                    AND StatusId = @AcceptedStatus";

            var parameters = new Dictionary<string, object>()
            {
                { "@UserId", userId },
                { "@EventId", personalEventId },
                { "@AcceptedStatus", InviteStatusHelper.Accepted }
            };

            var result = (int)await _dbHelper.ExecuteScalarAsync(hasUserAcceptedInviteQuery, parameters);
            return result > 0;
        }
        public async Task<bool> InviteExistsAsync(int eventId, int invitedUserId)
        {
            string query = "SELECT COUNT(*) FROM Invite WHERE PersonalEventId = @EventId AND InvitedUserId = @UserId";

            var parameters = new Dictionary<string, object>
            {
                { "@EventId", eventId },
                { "@UserId", invitedUserId }
            };

            var result = await _dbHelper.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }
        public async Task MarkInviteAsLeftAsync(int userId, int eventId)
        {
            string markInviteAsDeclinedQuery = @"
                UPDATE Invite
                SET StatusId = @DeclinedStatus
                WHERE InvitedUserId = @UserId 
                AND PersonalEventId = @EventId 
                AND StatusId = @AcceptedStatus";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@EventId", eventId },
                { "@DeclinedStatus", InviteStatusHelper.Declined },
                { "@AcceptedStatus", InviteStatusHelper.Accepted }
            };

            await _dbHelper.ExecuteNonQueryAsync(markInviteAsDeclinedQuery, parameters);

        }
        public async Task UpdateInviteStatusAsync(int inviteId, int statusId)
        {
            string updateInviteStatusQuery = "UPDATE Invite SET StatusId = @StatusId WHERE Id = @Id";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", inviteId },
                { "@StatusId", statusId }
            };

            await _dbHelper.ExecuteNonQueryAsync(updateInviteStatusQuery, parameters);
        }
        private async Task<(int Id, int StatusId)?> GetInviteStatusAndIdAsync(int eventId, int userId)
        {
            var getInviteStatusQuery = "SELECT Id, StatusId FROM Invite WHERE PersonalEventId = @EventId AND InvitedUserId = @UserId";
            var parameters = new Dictionary<string, object>
            {
                { "@EventId", eventId },
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getInviteStatusQuery, parameters);
            if (dt.Rows.Count == 0) return null;

            return (Convert.ToInt32(dt.Rows[0]["Id"]), Convert.ToInt32(dt.Rows[0]["StatusId"]));
        }
        private async Task InsertInviteAsync(Invite invite)
        {
            var insertInviteQuery = @"
                INSERT INTO Invite (PersonalEventId, InvitedUserId, StatusId, CreatedAt)
                VALUES (@PersonalEventId, @InvitedUserId, @StatusId, @CreatedAt)";

            var parameters = new Dictionary<string, object>
            {
                { "@PersonalEventId", invite.PersonalEventId },
                { "@InvitedUserId", invite.InvitedUserId },
                { "@StatusId", InviteStatusHelper.Pending },
                { "@CreatedAt", DateTime.UtcNow }
            };

            await _dbHelper.ExecuteNonQueryAsync(insertInviteQuery, parameters);
        }
        private async Task ResetInviteToPendingAsync(int inviteId)
        {
            var resetInviteQuery = "UPDATE Invite SET StatusId = @StatusId, CreatedAt = @CreatedAt WHERE Id = @Id";
            var parameters = new Dictionary<string, object>
            {
                { "@Id", inviteId },
                { "@StatusId", InviteStatusHelper.Pending },
                { "@CreatedAt", DateTime.UtcNow }
            };

            await _dbHelper.ExecuteNonQueryAsync(resetInviteQuery, parameters);
        }
    }
}
