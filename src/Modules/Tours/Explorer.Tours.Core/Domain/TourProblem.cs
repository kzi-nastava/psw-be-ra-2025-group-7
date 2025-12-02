using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum ProblemCategory { Safety, Navigation, Equipment, Other }
    public enum ProblemPriority { Low, Medium, High }
    public enum ProblemStatus { Open,  Resolved, Unresolved }
    public class TourProblem:AggregateRoot
    {

        public int TourId { get; private set; }
        public int TouristId { get; private set; }
        public ProblemCategory Category { get; private set; }
        public ProblemPriority Priority { get; private set; }
        public string Description { get; private set; }
        public DateTime TimeReported { get; private set; }
        public ProblemStatus Status { get; private set; } = ProblemStatus.Open;

        private readonly List<TourProblemMessage> _comments = new();
        public IReadOnlyCollection<TourProblemMessage> Comments => _comments;

        private TourProblem() { }
        public TourProblem(int tourId, int touristId, ProblemCategory category, ProblemPriority priority, string description)
        {
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");
            TourId = tourId;
            TouristId = touristId;
            Category = category;
            Priority = priority;
            Description = description;
            TimeReported = DateTime.UtcNow;
            Status = ProblemStatus.Open;

            _comments.Add(new TourProblemMessage(TouristId, Description, TimeReported));

        }

        public void AddAuthorReply(int authorId, string message)
        {
            if (Status == ProblemStatus.Resolved)
                throw new InvalidOperationException("Cannot reply to resolved problem.");
            if(string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.");
            var comment = new TourProblemMessage(authorId, message);
            _comments.Add(comment);
        }

        public void MarkAsResolved(int touristId)
        {
            if (TouristId != touristId)
                throw new InvalidOperationException("Only the reporting tourist can resolve the problem.");


            Status = ProblemStatus.Resolved;
        }
        public void MarkAsNotResolved(int touristId, string comment)
        {
            if (TouristId != touristId)
                throw new InvalidOperationException("Only the reporting tourist can respond.");
            if(string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty when marking as not resolved.");
            if (Status != ProblemStatus.Resolved)
                throw new InvalidOperationException("Response cannot be changed.");

            _comments.Add(new TourProblemMessage(touristId, comment));
            Status = ProblemStatus.Unresolved;
        }


    }
}
