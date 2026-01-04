using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Tours.Core.Domain
{
    public class TourRequest : Entity
    {
        public long TouristId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public int? Radius { get; private set; }

        public decimal Budget { get; private set; }

        public TourDifficulty? PreferredDifficulty { get; private set; }
        public int NumberOfParticipants { get; private set; }
        public DateTime? PreferredDate { get; private set; }

        public TourRequestStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        private TourRequest() { } 

        public TourRequest(long touristId, string title, string description, decimal budget)
        {
            ValidateTitle(title);
            ValidateDescription(description);
            ValidateBudget(budget);

            TouristId = touristId;
            Title = title.Trim();
            Description = description.Trim();
            Budget = budget;

            NumberOfParticipants = 1;
            Status = TourRequestStatus.Open;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddDays(30);
        }

        public void SetLocation(double latitude, double longitude, int radius)
        {
            ValidateRadius(radius);
            Latitude = latitude;
            Longitude = longitude;
            Radius = radius;
        }

        public void ClearLocation()
        {
            Latitude = null;
            Longitude = null;
            Radius = null;
        }

        public void SetPreferredDifficulty(TourDifficulty difficulty)
        {
            PreferredDifficulty = difficulty;
        }

        public void ClearPreferredDifficulty()
        {
            PreferredDifficulty = null;
        }

        public void SetNumberOfParticipants(int count)
        {
            if (count < 1)
                throw new ArgumentException("Number of participants must be at least 1.");
            NumberOfParticipants = count;
        }

        public void SetPreferredDate(DateTime date)
        {
            if (date < DateTime.UtcNow)
                throw new ArgumentException("Preferred date cannot be in the past.");
            PreferredDate = date;
        }

        public void ClearPreferredDate()
        {
            PreferredDate = null;
        }

        public void Update(string title, string description, decimal budget)
        {
            EnsureOpenStatus();

            ValidateTitle(title);
            ValidateDescription(description);
            ValidateBudget(budget);

            Title = title.Trim();
            Description = description.Trim();
            Budget = budget;
        }

        public void MarkInProgress()
        {
            if (Status == TourRequestStatus.Open)
            {
                Status = TourRequestStatus.InProgress;
            }
        }

        public void MarkFulfilled()
        {
            if (Status == TourRequestStatus.Open || Status == TourRequestStatus.InProgress)
            {
                Status = TourRequestStatus.Fulfilled;
            }
        }

        public void Close()
        {
            if (Status != TourRequestStatus.Fulfilled)
            {
                Status = TourRequestStatus.Closed;
            }
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt && Status == TourRequestStatus.Open;
        }

        public int DaysUntilExpiration()
        {
            if (Status != TourRequestStatus.Open)
                return 0;

            var days = (ExpiresAt - DateTime.UtcNow).Days;
            return days > 0 ? days : 0;
        }

        private void EnsureOpenStatus()
        {
            if (Status != TourRequestStatus.Open)
                throw new InvalidOperationException("This request cannot be edited.");
        }

        private void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");
            if (title.Trim().Length < 10)
                throw new ArgumentException("Title must be at least 10 characters.");
        }

        private void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.");
            if (description.Trim().Length < 50)
                throw new ArgumentException("Description must be at least 50 characters.");
        }

        private void ValidateBudget(decimal budget)
        {
            if (budget <= 0)
                throw new ArgumentException("Budget must be greater than zero.");
        }

        private void ValidateRadius(int radius)
        {
            if (radius < 1 || radius > 100)
                throw new ArgumentException("Radius must be between 1 and 100 km.");
        }
    }

    public enum TourRequestStatus
    {
        Open,
        InProgress,
        Fulfilled,
        Closed
    }
}