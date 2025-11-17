using Explorer.BuildingBlocks.Core.Domain;
using System.Net.Mail;

namespace Explorer.Tours.Core.Domain
{
    public class Tour : Entity
    {
        public long AuthorId { get; init; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TourDifficulty Difficulty { get; private set; }
        public List<string>Tags { get; private set; }
        public TourStatus Status { get; private set; }
        public decimal Price { get; private set; }

        public Tour(long authorId, string name, string description, TourDifficulty difficulty, List<string> tags)
        {
            AuthorId = authorId;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Status = TourStatus.Draft;
            Price = 0;
            Validate();
        }

        private void Validate()
        {
            ValidateName();
            ValidateDescription();
            ValidateTags();
        }

        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Tour name is required.");
            if (Name.Length > 100)
                throw new ArgumentException("Tour name cannot exceed 100 characters.");
        }

        private void ValidateDescription()
        {
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Tour description is required.");
            if (Description.Length > 1000)
                throw new ArgumentException("Tour description cannot exceed 1000 characters.");
        }

        private void ValidateTags()
        {
            if (Tags.Count > 10)
                throw new ArgumentException("Maximum 10 tags allowed.");
            if (Tags.Any(tag => string.IsNullOrWhiteSpace(tag)))
                throw new ArgumentException("Tags cannot be empty.");
            if (Tags.Any(tag => tag.Length > 50))
                throw new ArgumentException("Tag cannot exceed 50 characters.");
        }
    }

    public enum TourDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum TourStatus
    {
        Draft,
        Published,
        Archived
    }
}
