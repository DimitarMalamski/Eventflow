namespace Eventflow.Domain.Exceptions
{
    public class InvalidLoginInputException : Exception
    {
        public InvalidLoginInputException(string message) : base(message) { }
    }
}
