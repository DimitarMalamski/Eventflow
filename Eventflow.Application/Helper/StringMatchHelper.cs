namespace Eventflow.Application.Helper
{
    public static class StringMatchHelper
    {
        public static bool Match(string? source, string? keyword)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   !string.IsNullOrWhiteSpace(keyword) &&
                   source.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
