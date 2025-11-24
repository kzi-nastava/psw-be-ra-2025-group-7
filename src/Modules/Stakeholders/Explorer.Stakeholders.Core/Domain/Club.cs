using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long CreatedBy { get; init; }
        public List<string> ImageUrls { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Club(string name, string description, long createdBy, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            CreatedBy = createdBy;
            ImageUrls = imageUrls ?? new List<string>();

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Invalid Name");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Invalid Description");

            if (CreatedBy <= 0)
                throw new ArgumentException("Invalid CreatedBy");

            if (ImageUrls == null || ImageUrls.Count == 0)
                throw new ArgumentException("At least one image is required");
        }

        public void Update(string name, string description, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            ImageUrls = imageUrls ?? new List<string>();

            UpdatedAt = DateTime.UtcNow;

            Validate();
        }
    }
}
