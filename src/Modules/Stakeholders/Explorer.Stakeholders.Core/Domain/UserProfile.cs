using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class UserProfile : Entity
    {
        public long UserId { get; init; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? ProfilePicture { get; private set; }
        public string? Biography { get; private set; }
        public string? Motto { get; private set; }
        public double? CurrentLatitude { get; private set; }
        public double? CurrentLongitude { get; private set; }
        public List<int>? FinishedEncounters { get; set; }


        public UserProfile(long userId, string firstName, string lastName,
                           string? profilePicture = null, string? biography = null, string? motto = null)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            ProfilePicture = profilePicture;
            Biography = biography;
            Motto = motto;
            Validate();
        }

        public void UpdateProfile(string firstName, string lastName,
                                 string? profilePicture, string? biography, string? motto)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateBiography(biography);
            ValidateMotto(motto);

            FirstName = firstName;
            LastName = lastName;
            ProfilePicture = profilePicture;
            Biography = biography;
            Motto = motto;
        }

        public void UpdateLocation(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Invalid Latitude");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid Longitude");

            CurrentLatitude = latitude;
            CurrentLongitude = longitude;
        }


        private void Validate()
        {
            if (UserId == 0) throw new ArgumentException("Invalid UserId");
            ValidateFirstName(FirstName);
            ValidateLastName(LastName);
            ValidateBiography(Biography);
            ValidateMotto(Motto);
            if (CurrentLatitude is not null && (CurrentLatitude < -90 || CurrentLatitude > 90))
                throw new ArgumentException("Invalid Latitude");

            if (CurrentLongitude is not null && (CurrentLongitude < -180 || CurrentLongitude > 180))
                throw new ArgumentException("Invalid Longitude");

        }

        private void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("FirstName cannot be empty");

            if (!IsValidName(firstName))
                throw new ArgumentException("FirstName can only contain letters and spaces");
        }

        private void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("LastName cannot be empty");

            if (!IsValidName(lastName))
                throw new ArgumentException("LastName can only contain letters and spaces");
        }

        private void ValidateBiography(string? biography)
        {
            if (biography != null && biography.Length > 500)
                throw new ArgumentException("Biography cannot exceed 500 characters");
        }

        private void ValidateMotto(string? motto)
        {
            if (motto != null && motto.Length > 150)
                throw new ArgumentException("Motto cannot exceed 150 characters");
        }

        private bool IsValidName(string name)
        {
            return name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }
    }
}
