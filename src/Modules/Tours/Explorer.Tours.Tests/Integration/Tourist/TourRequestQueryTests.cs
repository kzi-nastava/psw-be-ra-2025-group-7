using Explorer.API.Controllers.Tourist.Tours;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourRequestQueryTests : BaseToursIntegrationTest
{
    public TourRequestQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_requests_for_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetMyRequests(1, 10);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(tr => tr.TouristId == -21);
    }

    [Fact]
    public void Retrieves_requests_with_pagination()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetMyRequests(1, 2);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeLessThanOrEqualTo(2);
    }

    [Fact]
    public void Retrieves_request_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetById(-1);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.TouristId.ShouldBe(-21);
        result.Title.ShouldNotBeNullOrEmpty();
        result.Status.ShouldBe(0); // Open
    }

    [Fact]
    public void Get_by_id_fails_for_different_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerForDifferentTourist(scope);

        // Act
        var actionResult = controller.GetById(-1);

        // Assert - trebalo bi da vrati Forbid (403)
        actionResult.Result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Retrieves_responses_for_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Request -1 has 2 responses
        var actionResult = controller.GetResponses(-1);
        var result = ((ObjectResult)actionResult.Result)?.Value as List<TourRequestResponseDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(r => r.TourRequestId == -1);
    }

    [Fact]
    public void Get_responses_fails_for_different_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerForDifferentTourist(scope);

        // Act
        var actionResult = controller.GetResponses(-1);

        // Assert - trebalo bi da vrati Forbid (403)
        actionResult.Result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Retrieves_correct_response_count()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetById(-1);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.ResponseCount.ShouldBe(2); // Request -1 has 2 responses
    }

    [Fact]
    public void Retrieves_correct_days_until_expiration()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Request -6 expires in 3 days
        var actionResult = controller.GetById(-6);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.DaysUntilExpiration.ShouldBeInRange(2, 4); // Allow small variance for test execution time
    }

    [Fact]
    public void Retrieves_zero_days_for_closed_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetById(-5);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(3); // Closed
        result.DaysUntilExpiration.ShouldBe(0);
    }

    [Fact]
    public void Returns_empty_list_for_tourist_without_requests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-98"); // Tourist without requests

        // Act
        var actionResult = controller.GetMyRequests(1, 10);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public void Retrieves_requests_with_different_statuses()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetMyRequests(1, 20);
        var allRequests = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourRequestDto>;

        // Assert
        allRequests.ShouldNotBeNull();
        allRequests.Results.ShouldContain(r => r.Status == 0); // Open
        allRequests.Results.ShouldContain(r => r.Status == 3); // Closed
    }

    private static TourRequestController CreateController(IServiceScope scope, string touristId = "-21")
    {
        return new TourRequestController(scope.ServiceProvider.GetRequiredService<ITourRequestService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }

    private static TourRequestController CreateControllerForDifferentTourist(IServiceScope scope)
    {
        return CreateController(scope, "-99");
    }
}