namespace DevJourney.Application.Dto.Common
{
    /// <summary>
    /// Author DTO representation.
    /// </summary>
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; }
    }
}
