using System;
using DevJourney.Domain.Common;

namespace DevJourney.Domain.Entities
{
    public class Author : BaseEntity
    {
        public string Name { get; private set; }
        public string? Avatar { get; private set; }
        public string? Role { get; private set; }
        public string? Bio { get; private set; }

        public Author(int id, string name, string? avatar = null, string? role = null, string? bio = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Author name is required", nameof(name));
            Id = id;
            Name = name;
            Avatar = avatar;
            Role = role;
            Bio = bio;
        }

        public void UpdateProfile(string name, string? avatar, string? role, string? bio)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Author name is required", nameof(name));
            Name = name;
            Avatar = avatar;
            Role = role;
            Bio = bio;
        }
    }
}
