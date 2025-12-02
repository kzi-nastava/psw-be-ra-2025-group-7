using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum PublicPointRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class PublicPointRequest : Entity
    {
        public long TourId { get; private set; }
        public int KeyPointIndex { get; private set; }   // pozicija tačke u listi KeyPoints
        public long AuthorId { get; private set; }

        public PublicPointRequestStatus Status { get; private set; }
        public string? AdminComment { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        // Za EF
        private PublicPointRequest() { }

        public PublicPointRequest(long tourId, int keyPointIndex, long authorId)
        {
            if (tourId <= 0) throw new ArgumentException("Invalid tour id.", nameof(tourId));
            if (keyPointIndex < 0) throw new ArgumentException("Invalid key point index.", nameof(keyPointIndex));
            if (authorId <= 0) throw new ArgumentException("Invalid author id.", nameof(authorId));

            TourId = tourId;
            KeyPointIndex = keyPointIndex;
            AuthorId = authorId;

            Status = PublicPointRequestStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            ProcessedAt = null;
            AdminComment = null;
        }

        // Ovo će koristiti kolega koji radi admin deo:
        public void Approve(string? comment = null)
        {
            if (Status != PublicPointRequestStatus.Pending)
                throw new InvalidOperationException("Request is already processed.");

            Status = PublicPointRequestStatus.Approved;
            AdminComment = comment;
            ProcessedAt = DateTime.UtcNow;
        }

        public void Reject(string comment)
        {
            if (Status != PublicPointRequestStatus.Pending)
                throw new InvalidOperationException("Request is already processed.");

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Rejection comment is required.", nameof(comment));

            Status = PublicPointRequestStatus.Rejected;
            AdminComment = comment.Trim();
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
