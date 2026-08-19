namespace DevJourney.Application.Exceptions
{
    /// <summary>
    /// Raised when a resource conflicts with existing data (e.g., duplicate slug).
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
