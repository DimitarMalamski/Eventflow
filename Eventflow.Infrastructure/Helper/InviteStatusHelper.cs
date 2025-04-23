namespace Eventflow.Infrastructure.Helper
{
    public static class InviteStatusHelper
    {
        public const int Pending = 1;
        public const int Accepted = 2;
        public const int Declined = 3;

        private static readonly Dictionary<int, string> _names = new()
        {
            { 1, "Pending" },
            { 2, "Accepted" },
            { 3, "Declined" }
        };

        public static string GetName(int id) =>
            _names.TryGetValue(id, out var name) ? name : "Unknown";
    }
}
