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
        public List<string> Tags { get; private set; }
        public TourStatus Status { get; private set; }
        public decimal Price { get; private set; }

        //Tvoja kartica 3 – kolekcija ključnih tačaka
        public List<KeyPoint> KeyPoints { get; private set; } = new();

        // TODO (Član 3): Pri objavi ture koristiće KeyPoints
        // - da proveri da li tura ima bar dve tačke.
        // - da vrati samo prvu tačku turistu.
        // Ovo je za preostale clanove, ne implementiram Publish logiku ovde.

        public Tour(long authorId, string name, string description, TourDifficulty difficulty, List<string> tags)
        {
            AuthorId = authorId;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Status = TourStatus.Draft;
            Price = 0;
            KeyPoints = new List<KeyPoint>();

            Validate();
        }

        //Javne metode za rad sa ključnim tačkama (Kartica 3)
        public void AddKeyPoint(KeyPoint keyPoint)
        {
            EnsureDraftStatus(); // zavisimo od toga da je član 1 dobro modelovao status

            if (keyPoint == null)
                throw new ArgumentNullException(nameof(keyPoint));

            KeyPoints.Add(keyPoint);
            // TODO (Član 3 / drugi): ovde mogu dodati logiku koja reaguje
            // na promenu putanje (npr. ponovno računanje dužine ture u Kartici 4).
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

        // Helper koji osigurava da kartica 3 važi samo za ture u pripremi
        private void EnsureDraftStatus()
        {
            // Ovde zavisim od člana 1:
            // oni su definisali TourStatus i logiku životnog ciklusa ture.
            if (Status != TourStatus.Draft)
                throw new InvalidOperationException("Key points can only be modified while tour is in Draft status.");
        }

        private void Validate()
        {
            ValidateName();
            ValidateDescription();
            ValidateTags();
            // Ostavljen hook ako jednog dana budeš validirao i KeyPoints
            // (trenutno specifikacija ne traži dodatna ograničenja).
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
