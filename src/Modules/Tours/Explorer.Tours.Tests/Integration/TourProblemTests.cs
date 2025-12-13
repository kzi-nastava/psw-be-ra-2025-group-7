using System;
using System.Linq;
using Xunit;
using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Integration
{
    public class TourProblemTests
    {
        // -----------------------------
        // Constructor Tests
        // -----------------------------
        [Fact]
        public void Constructor_creates_problem_successfully()
        {
            // Arrange
            int tourId = 10;
            int touristId = 20;
            string description = "Problem description.";

            // Act
            var problem = new TourProblem(
                tourId,
                touristId,
                ProblemCategory.Safety,
                ProblemPriority.High,
                description,
                false
            );

            // Assert
            problem.TourId.ShouldBe(tourId);
            problem.TouristId.ShouldBe(touristId);
            problem.Status.ShouldBe(ProblemStatus.Open);
            problem.Comments.Count.ShouldBe(1);
            problem.Comments.First().Message.ShouldBe(description);
            problem.Comments.First().CreatorId.ShouldBe(touristId);
        }

        [Fact]
        public void Constructor_throws_on_invalid_description()
        {
            Should.Throw<ArgumentException>(() =>
            {
                new TourProblem(
                    1,
                    1,
                    ProblemCategory.Other,
                    ProblemPriority.Low,
                    " ",
                    false
                );
            });
        }

        // -----------------------------
        // AddAuthorReply Tests
        // -----------------------------

        [Fact]
        public void AddAuthorReply_adds_comment()
        {
            // Arrange
            var problem = CreateSampleProblem();

            string reply = "We are examining the issue.";
            int authorId = 999;

            // Act
            problem.AddAuthorReply(authorId, reply);

            // Assert
            problem.Comments.Last().Message.ShouldBe(reply);
            problem.Comments.Last().CreatorId.ShouldBe(authorId);
        }

        [Fact]
        public void AddAuthorReply_throws_on_empty_message()
        {
            var problem = CreateSampleProblem();

            Should.Throw<ArgumentException>(() =>
            {
                problem.AddAuthorReply(1, " ");
            });
        }
        [Fact]
        public void MarkAsResolved_sets_status_to_resolved()
        {
            // Arrange
            var problem = CreateSampleProblem();
            int touristId = problem.TouristId;

            // Act
            problem.MarkAsResolved(touristId);

            // Assert
            problem.Status.ShouldBe(ProblemStatus.Resolved);
        }
        [Fact]
        public void MarkAsResolved_throws_when_not_creator()
        {
            var problem = CreateSampleProblem();

            Should.Throw<InvalidOperationException>(() =>
            {
                problem.MarkAsResolved(9999); // pogrešan korisnik
            });
        }
        [Fact]
        public void MarkAsNotResolved_adds_comment_and_sets_status()
        {
            // Arrange
            var problem = CreateSampleProblem();
            int touristId = problem.TouristId;
            string comment = "This is still NOT fixed.";

            // Act
            problem.MarkAsNotResolved(touristId, comment);

            // Assert
            problem.Status.ShouldBe(ProblemStatus.Unresolved);
            problem.Comments.Last().Message.ShouldBe(comment);
            problem.Comments.Last().CreatorId.ShouldBe(touristId);
        }
        [Fact]
        public void MarkAsNotResolved_throws_if_already_resolved()
        {
            // Arrange
            var problem = CreateSampleProblem();
            int touristId = problem.TouristId;

            problem.MarkAsResolved(touristId);

            // Act + Assert
            Should.Throw<InvalidOperationException>(() =>
            {
                problem.MarkAsNotResolved(touristId, "Still broken!");
            });
        }
        [Fact]
        public void MarkAsNotResolved_throws_on_empty_comment()
        {
            var problem = CreateSampleProblem();
            int touristId = problem.TouristId;

            Should.Throw<ArgumentException>(() =>
            {
                problem.MarkAsNotResolved(touristId, " ");
            });
        }
        // -----------------------------
        // Helpers
        // -----------------------------

        private TourProblem CreateSampleProblem()
        {
            return new TourProblem(
                10,
                20,
                ProblemCategory.Safety,
                ProblemPriority.High,
                "Initial description",
                false
            );
        }
    }
}