using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourReviewIntegrationTests : BaseToursIntegrationTest
{
    public TourReviewIntegrationTests(ToursTestFactory factory) : base(factory) { }

    #region CreateReview Tests

    [Fact]
    public void CreateReview_Creates_New_Review_With_Progress_Tracking()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2"); // Tourist -2 has active execution with > 35% progress
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new CreateTourReviewDto
        {
            TourId = -3,
            TourExecutionId = -13, // Tourist -2, execution with 66.7% progress (> 35%)
            Rating = 5,
            Comment = "Amazing tour! The experience was unforgettable and well worth it.",
            ImageUrls = new List<string>
            {
                "https://example.com/review-photo1.jpg",
                "https://example.com/review-photo2.jpg"
            }
        };

        // Act
        var result = ((ObjectResult)controller.CreateReview(dto).Result)?.Value as TourReviewDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TouristId.ShouldBe(-2);
        result.TourId.ShouldBe(-3);
        result.Rating.ShouldBe(5);
        result.Comment.ShouldContain("Amazing tour");
        result.TourProgressPercentage.ShouldBeGreaterThan(35.0); // Must be > 35%
        result.CreatedAt.ShouldNotBe(default(DateTime));
        result.UpdatedAt.ShouldBeNull();
        result.ImageUrls.Count.ShouldBe(2);

        // Assert - Database
        var storedReview = dbContext.TourReviews.FirstOrDefault(tr => tr.Id == result.Id);
        storedReview.ShouldNotBeNull();
        storedReview.TourProgressPercentage.ShouldBe(result.TourProgressPercentage);
    }

    [Fact]
    public void CreateReview_Fails_When_Tour_Not_Purchased()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-99"); // Tourist -99 hasn't purchased tour -3

        var dto = new CreateTourReviewDto
        {
            TourId = -3,
            TourExecutionId = -1,
            Rating = 5,
            Comment = "Great tour"
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.CreateReview(dto))
            .Message.ShouldContain("must purchase the tour");
    }

    [Fact]
    public void CreateReview_Fails_When_Progress_Less_Than_35_Percent()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2"); // Tourist -2

        // Execution -14 has only 1 keypoint unlocked (33.3% < 35%)
        var dto = new CreateTourReviewDto
        {
            TourId = -3,
            TourExecutionId = -14, // Tourist -2, only 33.3% progress
            Rating = 4,
            Comment = "Good so far"
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.CreateReview(dto))
            .Message.ShouldContain("must complete at least 35%");
    }

    [Fact]
    public void CreateReview_Fails_When_Already_Reviewed()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1"); // Tourist -1 already has a review for tour -3

        var dto = new CreateTourReviewDto
        {
            TourId = -3,
            TourExecutionId = -1,
            Rating = 4,
            Comment = "Another review"
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.CreateReview(dto))
            .Message.ShouldContain("already reviewed");
    }

    #endregion

    #region UpdateReview Tests

    [Fact]
    public void UpdateReview_Fails_When_Not_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2"); // Review -1 belongs to tourist -1

        var dto = new UpdateTourReviewDto
        {
            Rating = 3,
            Comment = "Trying to update someone else's review"
        };

        // Act & Assert
        Should.Throw<ForbiddenException>(() => controller.UpdateReview(-1, dto))
            .Message.ShouldContain("only update your own");
    }

    #endregion

    #region DeleteReview Tests

    [Fact]
    public void DeleteReview_Removes_Review()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = controller.DeleteReview(-1);

        // Assert - Response
        result.ShouldBeOfType<OkResult>();

        // Assert - Database
        var deletedReview = dbContext.TourReviews.FirstOrDefault(tr => tr.Id == -1);
        deletedReview.ShouldBeNull();
    }

    [Fact]
    public void DeleteReview_Fails_When_Not_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2"); // Review -1 belongs to tourist -1

        // Act & Assert
        Should.Throw<ForbiddenException>(() => controller.DeleteReview(-1))
            .Message.ShouldContain("only delete your own");
    }

    #endregion

    #region GetMyReviewForTour Tests

    [Fact]
    public void GetMyReviewForTour_Returns_Tourist_Review_For_Specific_Tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetMyReviewForTour(-3).Result)?.Value as TourReviewDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.TouristId.ShouldBe(-1);
        result.TourId.ShouldBe(-3);
    }

    [Fact]
    public void GetMyReviewForTour_Throws_When_Review_Not_Found()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-99"); // Tourist -99 has no reviews

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.GetMyReviewForTour(-3));
    }

    #endregion

    #region GetReviewsForTour Tests

    [Fact]
    public void GetReviewsForTour_Returns_All_Reviews_For_Tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetReviewsForTour(-3, 1, 20).Result)?.Value as PagedResult<TourReviewDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(r => r.TourId == -3).ShouldBeTrue();
    }

    [Fact]
    public void GetReviewsForTour_Respects_Pagination()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetReviewsForTour(-3, 1, 10).Result)?.Value as PagedResult<TourReviewDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThan(0);
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void GetReviewsForTour_Returns_Reviews_With_Progress_Percentage()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetReviewsForTour(-3, 1, 10).Result)?.Value as PagedResult<TourReviewDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(r => r.TourProgressPercentage >= 0 && r.TourProgressPercentage <= 100).ShouldBeTrue();
        
        var completedReview = result.Results.FirstOrDefault(r => r.Id == -1);
        completedReview.ShouldNotBeNull();
        completedReview.TourProgressPercentage.ShouldBe(100.0); // Completed tour
    }

    #endregion

    #region GetMyReviews Tests

    [Fact]
    public void GetMyReviews_Returns_All_Tourist_Reviews()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetMyReviews(1, 20).Result)?.Value as PagedResult<TourReviewDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(r => r.TouristId == -1).ShouldBeTrue();
    }

    #endregion

    #region GetAverageRating Tests

    [Fact]
    public void GetAverageRating_Returns_Correct_Average()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - Tour -3 has reviews with ratings 5 and 4, average = 4.5
        var result = ((ObjectResult)controller.GetAverageRating(-3).Result)?.Value;

        // Assert
        result.ShouldNotBeNull();
        var averageRating = result.GetType().GetProperty("averageRating")?.GetValue(result) as double?;
        averageRating.ShouldNotBeNull();
        averageRating.Value.ShouldBe(4.5);
    }

    [Fact]
    public void GetAverageRating_Returns_Zero_When_No_Reviews()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - Tour -1 has no reviews
        var result = ((ObjectResult)controller.GetAverageRating(-1).Result)?.Value;

        // Assert
        result.ShouldNotBeNull();
        var averageRating = result.GetType().GetProperty("averageRating")?.GetValue(result) as double?;
        averageRating.ShouldNotBeNull();
        averageRating.Value.ShouldBe(0);
    }

    #endregion

    #region CanLeaveReview Tests

    [Fact]
    public void CanLeaveReview_Returns_True_When_Eligible()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1"); // Tourist -1

        // Act - Execution -4 has > 35% progress (2 of 3 keypoints = 66.7%)
        var result = ((ObjectResult)controller.CanLeaveReview(-4).Result)?.Value;

        // Assert
        result.ShouldNotBeNull();
        var canReview = result.GetType().GetProperty("canReview")?.GetValue(result) as bool?;
        canReview.ShouldNotBeNull();
        canReview.Value.ShouldBeTrue();
    }

    [Fact]
    public void CanLeaveReview_Returns_False_When_Progress_Too_Low()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2"); // Tourist -2

        // Act - Execution -14 has only 33.3% progress (1 of 3 keypoints)
        var result = ((ObjectResult)controller.CanLeaveReview(-14).Result)?.Value;

        // Assert
        result.ShouldNotBeNull();
        var canReview = result.GetType().GetProperty("canReview")?.GetValue(result) as bool?;
        canReview.ShouldNotBeNull();
        canReview.Value.ShouldBeFalse();
    }

    #endregion

    private static TourReviewController CreateController(IServiceScope scope, string userId)
    {
        return new TourReviewController(scope.ServiceProvider.GetRequiredService<ITourReviewService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
