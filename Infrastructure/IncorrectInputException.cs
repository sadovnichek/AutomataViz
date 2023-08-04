namespace Infrastructure
{
    public class IncorrectInputException : Exception
    {
        public IncorrectInputException(string message) : base(message) { }
    }
}