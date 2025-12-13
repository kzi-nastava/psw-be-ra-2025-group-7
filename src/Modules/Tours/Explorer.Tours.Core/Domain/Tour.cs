using Explorer.BuildingBlocks.Core.Domain;
using System.Net.Mail;

namespace Explorer.Tours.Core.Domain
{
    public class Tour : AggregateRoot
    {
        public long AuthorId { get; init; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TourDifficulty Difficulty { get; private set; }
        public List<string> Tags { get; private set; }
        public TourStatus Status { get; set; }
        public decimal Price { get; private set; }

        public DateTime? PublishedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }


        public List<KeyPoint> KeyPoints { get; private set; } = new();
        public List<TourDuration> TourDurations { get; private set; } = new();

        public List<Equipment> RequiredEquipment { get; private set; } = new();

        private Tour()
        {
            Tags = new List<string>();
            KeyPoints = new List<KeyPoint>();
            TourDurations = new List<TourDuration>();
            RequiredEquipment = new List<Equipment>();
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
            KeyPoints = new List<KeyPoint>();
            TourDurations = new List<TourDuration>();
            RequiredEquipment = new List<Equipment>();
            Validate();
        }

        public void Publish()
        {
            if (Status == TourStatus.Archived)
                throw new InvalidOperationException("Cannot publish archived tour. Reactivate it first.");

            if (!HasRequiredData())
                throw new InvalidOperationException("Cannot publish tour without all required data.");

            if (KeyPoints.Count < 2)
                throw new InvalidOperationException("Cannot publish tour with less than 2 key points.");

            if (TourDurations == null || !TourDurations.Any())
                throw new InvalidOperationException("Cannot publish tour without at least one duration.");

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

        // ================= KARTICA 3 – KLJUČNE TAČKE  =================

        public void AddKeyPoint(KeyPoint keyPoint)
        {
            EnsureDraftStatus(); // oslanja se na status koji je modelovao član 1

            if (keyPoint == null)
                throw new ArgumentNullException(nameof(keyPoint));

            KeyPoints.Add(keyPoint);
            // TODO (Član 3 / drugi): ovde kasnije mogu da računaju dužinu ture itd.
        }

        public void UpdateKeyPoint(int index, KeyPoint keyPoint)
        {
            EnsureDraftStatus();

            if (index < 0 || index >= KeyPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Key point index is out of range.");

            if (keyPoint == null)
                throw new ArgumentNullException(nameof(keyPoint));

            KeyPoints[index] = keyPoint;
        }

        public void RemoveKeyPoint(int index)
        {
            EnsureDraftStatus();

            if (index < 0 || index >= KeyPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Key point index is out of range.");

            KeyPoints.RemoveAt(index);
        }

        public void AddEquipment(Equipment equipment)
        {
            EnsureNotArchived();

            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            if (RequiredEquipment.Any(e => e.Id == equipment.Id))
                throw new InvalidOperationException("Equipment is already added to this tour.");

            RequiredEquipment.Add(equipment);
        }

        public void RemoveEquipment(long equipmentId)
        {
            EnsureNotArchived();

            var equipment = RequiredEquipment.FirstOrDefault(e => e.Id == equipmentId);

            if (equipment == null)
                throw new InvalidOperationException("Equipment is not part of this tour.");

            RequiredEquipment.Remove(equipment);
        }

        private void EnsureNotArchived()
        {
            if (Status == TourStatus.Archived)
                throw new InvalidOperationException("You cannot modify equipment for archived tours. Please reactivate the tour first.");
        }


        // Helper – kartica 3 važi samo dok je tura u pripremi
        private void EnsureDraftStatus()
        {
            if (Status != TourStatus.Draft)
                throw new InvalidOperationException("Key points can only be modified while tour is in Draft status.");
        }

        // ================= VALIDACIJA =================

        public void Validate()
        {
            ValidateName();
            ValidateDescription();
            ValidateTags();
            // Ovde se po potrebi može dodati validacija KeyPoints
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

        public void AddDuration(TourDuration duration)
        {
            EnsureDraftStatus();

            if (duration == null)
                throw new ArgumentNullException(nameof(duration));

            TourDurations.Add(duration);
        }

        public void UpdateDuration(int index, TourDuration duration)
        {
            EnsureDraftStatus();

            if (index < 0 || index >= TourDurations.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Duration index is out of range.");

            if (duration == null)
                throw new ArgumentNullException(nameof(duration));

            TourDurations[index] = duration;
        }

        public void RemoveDuration(int index)
        {
            EnsureDraftStatus();

            if (index < 0 || index >= TourDurations.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Duration index is out of range.");

            TourDurations.RemoveAt(index);
        }

        // ================= VALIDACIJA ZA KUPOVINU (SHOPPING CART) =================

        public void ValidatePurchase()
        {
            if (Status == TourStatus.Archived)
                throw new InvalidOperationException("Archived tours cannot be purchased.");

            if (Status != TourStatus.Published)
                throw new InvalidOperationException("Only published tours can be purchased.");
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
