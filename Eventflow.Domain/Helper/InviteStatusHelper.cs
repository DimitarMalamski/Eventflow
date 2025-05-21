namespace Eventflow.Domain.Helper
{
    public static class InviteStatusHelper
    {
        public const int Pending = 1;
        public const int Accepted = 2;
        public const int Declined = 3;
        public const int KickedOut = 4;

        private static readonly Dictionary<int, string> _names = new()
        {
            { 1, "Pending" },
            { 2, "Accepted" },
            { 3, "Declined" },
            { 4, "KickedOut" }
        };

        public static string GetName(int id) =>
            _names.TryGetValue(id, out var name) ? name : "Unknown";
    }
}
