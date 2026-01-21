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

        public static TourRequestResponse CreateCustomProposal(long tourRequestId, long authorId, long tourId, string proposalDescription,
                                                                decimal proposedPrice,string? message)
        {
            if (string.IsNullOrWhiteSpace(proposalDescription))
                throw new ArgumentException("Proposal description is required for custom proposals.");

            return new TourRequestResponse
            {
                TourRequestId = tourRequestId,
                AuthorId = authorId,
                ResponseType = ResponseType.CustomProposal,
                TourId = tourId,       
                ProposalDescription = proposalDescription,
                ProposedPrice = proposedPrice,
                Message = message,
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
            if (ResponseType == ResponseType.ExistingTour)
            {
                if (Status != ResponseStatus.Pending)
                    throw new InvalidOperationException("Can only accept pending responses.");
            }
            else
            {
                if (Status != ResponseStatus.Ready)
                    throw new InvalidOperationException("Custom proposal must be in Ready state before final acceptance.");
            }

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

        public void ExpressInterest()
        {
            if (Status != ResponseStatus.Pending)
                throw new InvalidOperationException("You can only express interest for pending responses.");

            if (ResponseType != ResponseType.CustomProposal)
                throw new InvalidOperationException("Interest can only be expressed for custom proposals.");

            Status = ResponseStatus.Interested;
        }

        public void MarkAsReady()
        {
            if (Status != ResponseStatus.Interested)
                throw new InvalidOperationException("Response must be in Interested state.");

            if (ResponseType != ResponseType.CustomProposal)
                throw new InvalidOperationException("Only custom proposals can be marked as ready.");

            if (!TourId.HasValue)
                throw new InvalidOperationException("Tour must be created before marking as ready.");

            Status = ResponseStatus.Ready;
        }
    }

    public enum ResponseType
    {
        ExistingTour,
        CustomProposal
    }

    public enum ResponseStatus
    {
        Pending = 0,      
        Interested = 1,   
        Ready = 2,        
        Accepted = 3,   
        Rejected = 4
    }
}
