namespace Eventflow.Domain.Exceptions
{
    public class UnauthorizedReminderAccessException : Exception
    {
        public UnauthorizedReminderAccessException(string message) : base(message) { }
    }
}
