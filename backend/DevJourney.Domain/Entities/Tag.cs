using System;
using DevJourney.Domain.Common;

namespace DevJourney.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; private set; }
        public string Slug { get; private set; }

        public Tag(int id, string name, string slug)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tag name is required", nameof(name));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Tag slug is required", nameof(slug));
            Id = id;
            Name = name;
            Slug = slug;
        }

        public void Update(string name, string slug)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tag name is required", nameof(name));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Tag slug is required", nameof(slug));
            Name = name;
            Slug = slug;
        }
    }
}
