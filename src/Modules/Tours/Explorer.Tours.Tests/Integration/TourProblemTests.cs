using System;
using System.Linq;
using Xunit;
using Explorer.Tours.Core.Domain;
using Shouldly;

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
