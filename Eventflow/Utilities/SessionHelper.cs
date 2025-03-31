namespace Eventflow.Utilities
{
    public static class SessionHelper
    {
        public static void SetUserSession(ISession session, int userId, string username, int roleId)
        {
            session.SetString("UserId", userId.ToString());
            session.SetString("Username", username);
            session.SetString("UserRoleId", roleId.ToString());
        }

        public static void ClearUserSession(ISession session)
        {
            session.Clear();
        }
        public static int GetUserId(ISession session)
        {
            return int.TryParse(session.GetString("UserId"), out int userId) ? userId : 0;
        }
        public static string GetUsername(ISession session)
        {
            return session.GetString("Username") ?? "Guest";
        }
        public static int GetUserRoleId(ISession session)
        {
            return int.TryParse(session.GetString("UserRoleId"), out int roleId) ? roleId : 0;
        }
        public static bool IsLoggedIn(ISession session)
        {
            return GetUserRoleId(session) != 0;
        }
    }
}
