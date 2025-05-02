namespace Eventflow.Domain.Exceptions
{
    public class ReminderNotFoundException : Exception
    {
        public ReminderNotFoundException(string message) : base(message) { }
    }
}
