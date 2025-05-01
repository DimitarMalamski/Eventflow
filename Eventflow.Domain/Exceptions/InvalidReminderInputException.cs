namespace Eventflow.Domain.Exceptions
{
    public class InvalidReminderInputException : Exception
    {
        public InvalidReminderInputException(string message) : base(message) { }
    }
}
