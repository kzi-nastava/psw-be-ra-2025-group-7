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

        public DateTime? PublishedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }

        public Tour()
        {
            Tags = new List<string>();
        }

        public Tour(long authorId, string name, string description, TourDifficulty difficulty, List<string> tags)
        {
            AuthorId = authorId;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Status = TourStatus.Draft;
            Price = 0;
            PublishedAt = null;
            ArchivedAt = null;
            Validate();
        }

        public void Publish()
        {
            if (Status == TourStatus.Archived)
                throw new InvalidOperationException("Cannot publish archived tour. Reactivate it first.");

            if (!HasRequiredData())
                throw new InvalidOperationException("Cannot publish tour without all required data.");

            Status = TourStatus.Published;
            PublishedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != TourStatus.Published)
                throw new InvalidOperationException("Only published tours can be archived.");

            if (Status == TourStatus.Archived)
                throw new InvalidOperationException("This tour is already archived.");

            Status = TourStatus.Archived;
            ArchivedAt = DateTime.UtcNow;
        }

        public void Reactivate(TourStatus newStatus)
        {
            if (Status != TourStatus.Archived)
                throw new InvalidOperationException("Only archived tours can be reactivated.");

            if (newStatus != TourStatus.Draft && newStatus != TourStatus.Published)
                throw new ArgumentException("Tour can only be reactivated as Draft or Published.");

            Status = newStatus;
            ArchivedAt = null;

            if (newStatus == TourStatus.Published)
                PublishedAt = DateTime.UtcNow;
        }

        public bool HasRequiredData()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && !string.IsNullOrWhiteSpace(Description)
                && Tags != null && Tags.Any();
        }

        public void Validate()
        {
            ValidateName();
            ValidateDescription();
            ValidateTags();
        }

        public void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Tour name is required.");
            if (Name.Length > 100)
                throw new ArgumentException("Tour name cannot exceed 100 characters.");
        }

        public void ValidateDescription()
        {
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Tour description is required.");
            if (Description.Length > 1000)
                throw new ArgumentException("Tour description cannot exceed 1000 characters.");
        }

        public void ValidateTags()
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
