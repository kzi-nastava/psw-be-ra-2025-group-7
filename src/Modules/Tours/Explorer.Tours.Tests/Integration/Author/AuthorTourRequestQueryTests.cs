using Explorer.API.Controllers.Author.Tours;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class AuthorTourRequestQueryTests : BaseToursIntegrationTest
{
    public AuthorTourRequestQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_open_requests_for_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 10, null, null);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.ShouldAllBe(r => r.Status == 0 || r.Status == 1); // Open or InProgress
    }

    [Fact]
    public void Filters_requests_by_min_budget()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 10, 15000, null);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(r => r.Budget >= 15000);
    }

    [Fact]
    public void Filters_requests_by_max_budget()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 10, null, 10000);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(r => r.Budget <= 10000);
    }

    [Fact]
    public void Filters_requests_by_budget_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 10, 8000, 20000);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(r => r.Budget >= 8000 && r.Budget <= 20000);
    }

    [Fact]
    public void Shows_already_responded_flag_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 10, null, null);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        var requestWithResponse = result.Results.FirstOrDefault(r => r.Id == -1);
        requestWithResponse.ShouldNotBeNull();
        requestWithResponse.AlreadyResponded.ShouldBeTrue(); // Author -1 has already responded to request -1
    }

    [Fact]
    public void Shows_response_count_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 10, null, null);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        var requestWithResponses = result.Results.FirstOrDefault(r => r.Id == -1);
        requestWithResponses.ShouldNotBeNull();
        requestWithResponses.ResponseCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Retrieves_request_details()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetDetails(-1);
        var result = ((ObjectResult)actionResult.Result)?.Value as AuthorTourRequestDetailsDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.TouristName.ShouldNotBeNullOrEmpty();
        result.ContactDisclaimer.ShouldContain("only get contact information");
        result.ResponseCount.ShouldBeGreaterThan(0);
        result.DaysUntilExpiration.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Get_details_fails_for_closed_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetDetails(-5); // Closed request

        // Assert
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Get_details_fails_for_fulfilled_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetDetails(-4); // Fulfilled request

        // Assert
        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Retrieves_my_responses()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetMyResponses();
        var result = ((ObjectResult)actionResult.Result)?.Value as List<AuthorResponseItemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(r => r.TouristName != null);
        result.ShouldAllBe(r => r.TourRequestTitle != null);
    }

    [Fact]
    public void My_responses_contain_tour_info_when_available()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetMyResponses();
        var result = ((ObjectResult)actionResult.Result)?.Value as List<AuthorResponseItemDto>;

        // Assert
        result.ShouldNotBeNull();
        var responseWithTour = result.FirstOrDefault(r => r.TourId.HasValue);
        if (responseWithTour != null)
        {
            responseWithTour.TourName.ShouldNotBeNullOrEmpty();
        }
    }

    [Fact]
    public void Open_requests_pagination_works()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetOpen(1, 2, null, null);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<AuthorTourRequestListItemDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeLessThanOrEqualTo(2);
    }

    private static AuthorTourRequestController CreateController(IServiceScope scope, string authorId = "-1")
    {
        return new AuthorTourRequestController(scope.ServiceProvider.GetRequiredService<IAuthorTourRequestService>())
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}