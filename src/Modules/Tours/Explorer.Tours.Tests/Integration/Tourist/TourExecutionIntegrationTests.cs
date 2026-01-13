using Explorer.API.Controllers.Tourist;
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
public class TourExecutionIntegrationTests : BaseToursIntegrationTest
{
    public TourExecutionIntegrationTests(ToursTestFactory factory) : base(factory) { }

    #region StartTour Tests

    [Fact]
    public void StartTour_Fails_When_Tour_Not_Purchased()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        var dto = new StartTourExecutionDto
        {
            TourId = -2, // Tour not purchased by tourist -1
            Latitude = 45.0,
            Longitude = 19.0
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.StartTour(dto))
            .Message.ShouldContain("Tour must be purchased before starting");
    }

    #endregion

    #region CompleteTour Tests

    [Fact]
    public void CompleteTour_Updates_Status_To_Completed()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act - using execution -1
        var result = ((ObjectResult)controller.CompleteTour(-1).Result)?.Value as TourExecutionDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Completed");
        result.CompletedAt.ShouldNotBeNull();

        // Verify in database
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == -1);
        storedExecution.ShouldNotBeNull();
        storedExecution.Status.ShouldBe(Core.Domain.TourExecutionStatus.Completed);
        storedExecution.CompletedAt.ShouldNotBeNull();
    }

    #endregion

    #region AbandonTour Tests

    [Fact]
    public void AbandonTour_Updates_Status_To_Abandoned()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act - using execution -2
        var result = ((ObjectResult)controller.AbandonTour(-2).Result)?.Value as TourExecutionDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Abandoned");
        result.AbandonedAt.ShouldNotBeNull();

        // Verify in database
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == -2);
        storedExecution.ShouldNotBeNull();
        storedExecution.Status.ShouldBe(Core.Domain.TourExecutionStatus.Abandoned);
    }

    #endregion

    #region CheckKeyPointProximity Tests

    [Fact]
    public void CheckKeyPointProximity_Unlocks_KeyPoint_When_Near()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new CheckKeyPointProximityDto
        {
            Latitude = 45.2551, // Near first keypoint of tour -3
            Longitude = 19.8636
        };

        // Act - using execution -3
        var result = ((ObjectResult)controller.CheckKeyPointProximity(-3, dto).Result)?.Value as KeyPointProximityCheckResultDto;

        // Assert
        result.ShouldNotBeNull();
        result.IsNearKeyPoint.ShouldBeTrue();
        result.KeyPointIndex.ShouldNotBeNull();
        result.KeyPointName.ShouldNotBeNull();
        result.DistanceInMeters.ShouldNotBeNull();
        result.DistanceInMeters.Value.ShouldBeLessThan(100.0); // Within 100 meters

        // Verify LastActivity was updated
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == -3);
        storedExecution.ShouldNotBeNull();
        storedExecution.LastActivity.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void CheckKeyPointProximity_Updates_LastActivity_Even_When_Not_Near()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var beforeCheck = DateTime.UtcNow;

        var dto = new CheckKeyPointProximityDto
        {
            Latitude = 50.0, // Far from any keypoint
            Longitude = 20.0
        };

        // Act - using execution -4
        var result = ((ObjectResult)controller.CheckKeyPointProximity(-4, dto).Result)?.Value as KeyPointProximityCheckResultDto;

        // Assert
        result.ShouldNotBeNull();
        result.IsNearKeyPoint.ShouldBeFalse();
        result.KeyPointIndex.ShouldBeNull();

        // Verify LastActivity was still updated
        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == -4);
        storedExecution.ShouldNotBeNull();
        storedExecution.LastActivity.ShouldBeGreaterThan(beforeCheck);
    }

    #endregion

    #region UpdateLastActivity Tests

    [Fact]
    public void UpdateLastActivity_Updates_Timestamp()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var beforeUpdate = DateTime.UtcNow;

        // Act - using execution -5
        var result = ((ObjectResult)controller.UpdateLastActivity(-5).Result)?.Value as TourExecutionDto;

        // Assert
        result.ShouldNotBeNull();

        var storedExecution = dbContext.TourExecutions.FirstOrDefault(te => te.Id == -5);
        storedExecution.ShouldNotBeNull();
        storedExecution.LastActivity.ShouldBeGreaterThan(beforeUpdate);
    }

    #endregion

    #region GetProgressPercentage Tests

    [Fact]
    public void GetProgressPercentage_Returns_Correct_Percentage()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - using execution -6 which has 1 out of 3 keypoints unlocked (33.3%)
        var result = ((ObjectResult)controller.GetProgressPercentage(-6).Result)?.Value;

        // Assert
        result.ShouldNotBeNull();
        var progressPercentage = result.GetType().GetProperty("progressPercentage")?.GetValue(result) as double?;
        progressPercentage.ShouldNotBeNull();
        progressPercentage.Value.ShouldBeInRange(32.0, 35.0); // ~33.3%
    }

    #endregion

    #region GetActiveExecution Tests

    [Fact]
    public void GetActiveExecution_Throws_When_No_Active_Execution()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act & Assert
        Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.NotFoundException>(() => 
            controller.GetActiveExecution(-1)); // Tour -1 has no active execution for tourist -1
    }

    #endregion

    #region GetExecutionHistory Tests

    [Fact]
    public void GetExecutionHistory_Returns_All_Executions_For_Tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetExecutionHistory().Result)?.Value as PagedResult<TourExecutionDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(te => te.TouristId == -1).ShouldBeTrue();
        result.Results.Count.ShouldBe(10); // Tourist -1 has 10 executions (IDs -1 through -10)
    }

    [Fact]
    public void GetExecutionHistory_Respects_Pagination()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetExecutionHistory(1, 1).Result)?.Value as PagedResult<TourExecutionDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(1);
        result.TotalCount.ShouldBe(10);
    }

    #endregion

    #region GetKeyPointSecret Tests

    [Fact]
    public void GetKeyPointSecret_Returns_Secret_When_KeyPoint_Unlocked()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - execution -8 has keypoint 0 unlocked
        var result = ((ObjectResult)controller.GetKeyPointSecret(-8, 0).Result)?.Value;

        // Assert
        result.ShouldNotBeNull();
        var secret = result.GetType().GetProperty("secret")?.GetValue(result) as string;
        secret.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void GetKeyPointSecret_Throws_When_KeyPoint_Not_Unlocked()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act & Assert - execution -9 does not have keypoint 2 unlocked
        Should.Throw<InvalidOperationException>(() => 
            controller.GetKeyPointSecret(-9, 1));
    }

    #endregion

    private static TourExecutionController CreateController(IServiceScope scope, string userId)
    {
        return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
