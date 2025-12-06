using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Unit.Clubs
{
    public class ClubAggregateTest
    {
        private static Club CreateClub()
        {
            return new Club(
                name: "Test klub",
                description: "Opis",
                createdBy: -21,
                imageUrls: new List<string> { "img.jpg" });
        }

        // ------------------------------
        // JOIN REQUEST – uspeh
        // ------------------------------
        [Fact]
        public void RequestMembership_creates_pending_request()
        {
            // Arrange
            var club = CreateClub();
            long touristId = 10;

            // Act
            club.RequestMembership(touristId);

            // Assert
            club.JoinRequests.Count.ShouldBe(1);
            club.JoinRequests.Single().TouristId.ShouldBe(touristId);
        }

        // ------------------------------
        // JOIN REQUEST – različiti fail slučajevi
        // ------------------------------
        [Theory]
        [InlineData(false, false, false)] // club closed
        [InlineData(true, true, false)] // already member
        [InlineData(true, false, true)]  // already has pending request
        public void RequestMembership_fails_in_invalid_states(
    bool active,
    bool alreadyMember,
    bool hasPendingRequest)
        {
            // Arrange
            var club = CreateClub();
            long touristId = 10;

            if (!active)
                club.Close();

            if (alreadyMember)
            {
                club.InviteTourist(touristId);
                club.AcceptInvitation(touristId);
            }

            if (hasPendingRequest)
                club.RequestMembership(touristId);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => club.RequestMembership(touristId));
        }

        // ------------------------------
        // INVITATION – unify test
        // ------------------------------
        [Fact]
        public void Invite_and_accept_invitation_adds_member()
        {
            // Arrange
            var club = CreateClub();
            long touristId = 15;

            // Act
            club.InviteTourist(touristId);

            // Assert 1
            club.Invitations.Count.ShouldBe(1);
            club.Members.Count.ShouldBe(0);

            // Act 2
            club.AcceptInvitation(touristId);

            // Assert 2
            club.Invitations.Count.ShouldBe(0);
            club.Members.Count.ShouldBe(1);
            club.Members.Single().TouristId.ShouldBe(touristId);
        }

        // ------------------------------
        // REMOVE MEMBER – fails & success
        // ------------------------------
        [Fact]
        public void RemoveMember_succeeds()
        {
            // Arrange
            var club = CreateClub();
            long touristId = 33;

            club.InviteTourist(touristId);
            club.AcceptInvitation(touristId);

            // Act
            club.RemoveMember(touristId);

            // Assert
            club.Members.Count.ShouldBe(0);
        }

        [Fact]
        public void RemoveMember_fails_when_member_not_exists()
        {
            // Arrange
            var club = CreateClub();

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => club.RemoveMember(999));
        }
    }
}
