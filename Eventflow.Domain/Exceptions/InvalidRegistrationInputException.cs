namespace Eventflow.Domain.Exceptions
{
    public class InvalidRegistrationInputException : Exception
    {
        public InvalidRegistrationInputException(string message) : base(message) { }
    }
}
