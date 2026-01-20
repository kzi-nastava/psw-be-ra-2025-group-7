using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long CreatedBy { get; private set; }

        public List<string> ImageUrls { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public ClubStatus Status { get; private set; }

        private readonly List<ClubMember> _members = new();
        public IReadOnlyCollection<ClubMember> Members => _members.AsReadOnly();

        private readonly List<ClubJoinRequest> _joinRequests = new();
        public IReadOnlyCollection<ClubJoinRequest> JoinRequests => _joinRequests.AsReadOnly();

        private readonly List<ClubInvitation> _invitations = new();
        public IReadOnlyCollection<ClubInvitation> Invitations => _invitations.AsReadOnly();

        private Club() { } // EF

        public Club(string name, string description, long createdBy, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            CreatedBy = createdBy;

            ImageUrls = NormalizeImageUrls(imageUrls);

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            Status = ClubStatus.Active;

            Validate();
        }

        public void Update(string name, string description, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            ImageUrls = NormalizeImageUrls(imageUrls);

            UpdatedAt = DateTime.UtcNow;
            Validate();
        }

        // ✅ New helper: safely append images without losing existing ones
        public void AddImages(IEnumerable<string> imageUrls)
        {
            var newUrls = NormalizeImageUrls(imageUrls);
            if (newUrls.Count == 0) return;

            ImageUrls ??= new List<string>();
            ImageUrls = ImageUrls
                .Concat(newUrls)
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Select(u => u.Trim())
                .Distinct()
                .ToList();

            UpdatedAt = DateTime.UtcNow;
        }

        private static List<string> NormalizeImageUrls(IEnumerable<string> imageUrls)
        {
            if (imageUrls == null) return new List<string>();

            return imageUrls
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Select(u => u.Trim())
                .Distinct()
                .ToList();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Club name is required.", nameof(Name));

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Club description is required.", nameof(Description));
        }

        public void Close() => Status = ClubStatus.Closed;
        public void Open() => Status = ClubStatus.Active;

        private void EnsureActive()
        {
            if (Status != ClubStatus.Active)
                throw new InvalidOperationException("Club is not active.");
        }

        public void RequestMembership(long touristId)
        {
            EnsureActive();

            if (_members.Any(m => m.TouristId == touristId))
                throw new InvalidOperationException("Tourist is already a member.");

            if (_joinRequests.Any(r => r.TouristId == touristId && r.Status == JoinRequestStatus.Pending))
                throw new InvalidOperationException("Pending join request already exists.");

            if (_invitations.Any(i => i.TouristId == touristId && i.Status == InvitationStatus.Sent))
                throw new InvalidOperationException("Tourist already has a pending invitation.");

            _joinRequests.Add(new ClubJoinRequest(touristId));
            UpdatedAt = DateTime.UtcNow;
        }

        public void WithdrawRequest(long touristId)
        {
            EnsureActive();

            var request = _joinRequests
                .SingleOrDefault(r => r.TouristId == touristId && r.Status == JoinRequestStatus.Pending);

            if (request == null)
                throw new InvalidOperationException("Pending join request not found.");

            _joinRequests.Remove(request);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AcceptRequest(long touristId)
        {
            EnsureActive();

            var request = _joinRequests
                .SingleOrDefault(r => r.TouristId == touristId && r.Status == JoinRequestStatus.Pending);

            if (request == null)
                throw new InvalidOperationException("Pending join request not found.");

            request.MarkAccepted();
            _joinRequests.Remove(request);
            _members.Add(new ClubMember(touristId));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RejectRequest(long touristId)
        {
            EnsureActive();

            var request = _joinRequests
                .SingleOrDefault(r => r.TouristId == touristId && r.Status == JoinRequestStatus.Pending);

            if (request == null)
                throw new InvalidOperationException("Pending join request not found.");

            request.MarkRejected();
            _joinRequests.Remove(request);
            UpdatedAt = DateTime.UtcNow;
        }

        public void InviteTourist(long touristId)
        {
            EnsureActive();

            if (_members.Any(m => m.TouristId == touristId))
                throw new InvalidOperationException("Tourist is already a member.");

            if (_joinRequests.Any(r => r.TouristId == touristId && r.Status == JoinRequestStatus.Pending))
                throw new InvalidOperationException("Tourist already has a pending join request.");

            if (_invitations.Any(i => i.TouristId == touristId && i.Status == InvitationStatus.Sent))
                throw new InvalidOperationException("Pending invitation already exists.");

            _invitations.Add(new ClubInvitation(touristId));
            UpdatedAt = DateTime.UtcNow;
        }

        public void AcceptInvitation(long touristId)
        {
            EnsureActive();

            var invitation = _invitations
                .SingleOrDefault(i => i.TouristId == touristId && i.Status == InvitationStatus.Sent);

            if (invitation == null)
                throw new InvalidOperationException("Invitation not found.");

            invitation.MarkAccepted();
            _invitations.Remove(invitation);
            _members.Add(new ClubMember(touristId));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RejectInvitation(long touristId)
        {
            EnsureActive();

            var invitation = _invitations
                .SingleOrDefault(i => i.TouristId == touristId && i.Status == InvitationStatus.Sent);

            if (invitation == null)
                throw new InvalidOperationException("Invitation not found.");

            invitation.MarkRejected();
            _invitations.Remove(invitation);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveMember(long touristId)
        {
            EnsureActive();

            var member = _members.SingleOrDefault(m => m.TouristId == touristId);
            if (member == null)
                throw new InvalidOperationException("Member not found.");

            _members.Remove(member);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
