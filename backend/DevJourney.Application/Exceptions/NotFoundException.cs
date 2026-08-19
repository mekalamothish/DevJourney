namespace DevJourney.Application.Exceptions
{
    /// <summary>
    /// Raised when a requested resource is not found.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string resourceType, int id)
            : base($"{resourceType} with ID {id} not found.")
        {
        }

        public NotFoundException(string resourceType, string identifier)
            : base($"{resourceType} '{identifier}' not found.")
        {
        }
    }
}
