using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Tours.Core.Domain
{
    public class TourRequestResponse : Entity
    {
        public long TourRequestId { get; private set; }
        public long AuthorId { get; private set; }
        public ResponseType ResponseType { get; private set; }

        public long? TourId { get; private set; }

        public string? ProposalDescription { get; private set; }
        public string? Message { get; private set; }

        public decimal ProposedPrice { get; private set; }
        public ResponseStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private TourRequestResponse() { }

        public static TourRequestResponse CreateForExistingTour(
            long tourRequestId,
            long authorId,
            long tourId,
            decimal proposedPrice,
            string message = null)
        {
            ValidatePrice(proposedPrice);

            

            return new TourRequestResponse
            {
                TourRequestId = tourRequestId,
                AuthorId = authorId,
                ResponseType = ResponseType.ExistingTour,
                TourId = tourId,
                ProposedPrice = proposedPrice,
                Message = message?.Trim(),
                Status = ResponseStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static TourRequestResponse CreateCustomProposal(
            long tourRequestId,
            long authorId,
            string proposalDescription,
            decimal proposedPrice,
            string message = null)
        {
            ValidateProposalDescription(proposalDescription);
            ValidatePrice(proposedPrice);

            return new TourRequestResponse
            {
                TourRequestId = tourRequestId,
                AuthorId = authorId,
                ResponseType = ResponseType.CustomProposal,
                ProposalDescription = proposalDescription.Trim(),
                ProposedPrice = proposedPrice,
                Message = message?.Trim(),
                Status = ResponseStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(decimal proposedPrice, string message = null)
        {
            EnsurePendingStatus();
            ValidatePrice(proposedPrice);

            ProposedPrice = proposedPrice;
            Message = message?.Trim();
        }

        public void UpdateProposalDescription(string proposalDescription)
        {
            EnsurePendingStatus();

            if (ResponseType != ResponseType.CustomProposal)
                throw new InvalidOperationException("Can only update proposal description for custom proposals.");

            ValidateProposalDescription(proposalDescription);
            ProposalDescription = proposalDescription.Trim();
        }

        public void UpdateTour(long tourId)
        {
            EnsurePendingStatus();

            if (ResponseType != ResponseType.ExistingTour)
                throw new InvalidOperationException("Can only update tour for existing tour responses.");

           

            TourId = tourId;
        }

        public void Accept()
        {
            EnsurePendingStatus();
            Status = ResponseStatus.Accepted;
        }

        public void Reject()
        {
            EnsurePendingStatus();
            Status = ResponseStatus.Rejected;
        }

        private void EnsurePendingStatus()
        {
            if (Status != ResponseStatus.Pending)
                throw new InvalidOperationException("You cannot edit accepted or rejected responses.");
        }

        private static void ValidateProposalDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Proposal description is required.");
        }

        private static void ValidatePrice(decimal price)
        {
            if (price <= 0)
                throw new ArgumentException("Proposed price must be greater than zero.");
        }
    }

    public enum ResponseType
    {
        ExistingTour,
        CustomProposal
    }

    public enum ResponseStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
