using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class KeyPoint : ValueObject
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public string Name { get; }
        public string Description { get; }
        public string? ImageUrl { get; }
        public string Secret { get; }

        // TODO (Drugi članovi): Po potrebi ovde mogu dodati dodatna polja
        // npr. redosled tačke u turi kada budu radili svoje kartice.

        public KeyPoint(
            double latitude,
            double longitude,
            string name,
            string description,
            string secret,
            string? imageUrl = null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
            Description = description;
            Secret = secret;
            ImageUrl = imageUrl;

            Validate();
        }

        private void Validate()
        {
            if (Latitude is < -90 or > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.");

            if (Longitude is < -180 or > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.");

            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Key point name is required.");

            if (Name.Length > 100)
                throw new ArgumentException("Key point name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Key point description is required.");

            if (Description.Length > 2000)
                throw new ArgumentException("Key point description cannot exceed 2000 characters.");

            if (string.IsNullOrWhiteSpace(Secret))
                throw new ArgumentException("Key point secret is required.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
            yield return Name;
            yield return Description;
            yield return ImageUrl ?? string.Empty;
            yield return Secret;
        }
    }
}
