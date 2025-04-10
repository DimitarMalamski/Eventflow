using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
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
    }
}
