namespace DevJourney.Application.Exceptions
{
    /// <summary>
    /// Raised when application-level validation fails.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string fieldName, string message)
            : base($"Validation failed for '{fieldName}': {message}")
        {
        }
    }
}
