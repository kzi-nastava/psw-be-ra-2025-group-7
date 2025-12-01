using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourLifecycleTests : BaseToursIntegrationTest
{
    public TourLifecycleTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Publish_Tour_Changes_Status_To_Published()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -500; // Draft tura ZA PUBLISH

        // Act
        var actionResult = controller.Publish(tourId).Result;
        actionResult.ShouldBeOfType<OkObjectResult>();
        var result = (actionResult as OkObjectResult)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Published
        result.PublishedAt.ShouldNotBeNull();
        result.ArchivedAt.ShouldBeNull();
    }

    [Fact]
    public void Archive_Published_Tour_Changes_Status_To_Archived()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -3; // Published tura ZA ARCHIVE

        // Act
        var actionResult = controller.Archive(tourId).Result;
        actionResult.ShouldBeOfType<OkObjectResult>();
        var result = (actionResult as OkObjectResult)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(2); // Archived
        result.ArchivedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Archive_Draft_Tour_Returns_BadRequest()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var tourId = -4; // RAZLIČITA Draft tura ZA ARCHIVE FAILURE TEST

        // Act
        var result = controller.Archive(tourId).Result;

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest.ShouldNotBeNull();
        badRequest.Value.ShouldBe("Only published tours can be archived.");
    }

    [Fact]
    public void Reactivate_Archived_Tour_As_Draft()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -5; // Published tura ZA REACTIVATE
        controller.Archive(tourId); // Prvo je arhiviraj

        var request = new ReactivateRequestDto { NewStatus = 0 }; // Draft

        // Act
        var actionResult = controller.Reactivate(tourId, request).Result;
        actionResult.ShouldBeOfType<OkObjectResult>();
        var result = (actionResult as OkObjectResult)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(0); // Draft
        result.ArchivedAt.ShouldBeNull();
    }

    [Fact]
    public void Reactivate_Archived_Tour_As_Published()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -3; // Već arhivirana tura iz prethodnog testa
        // AKO NIJE ARHIVIRANA, arhiviraj je:
        var tour = dbContext.Tours.Find((long)tourId);
        if (tour.Status != Core.Domain.TourStatus.Archived)
        {
            controller.Archive(tourId);
        }

        var request = new ReactivateRequestDto { NewStatus = 1 }; // Published

        // Act
        var actionResult = controller.Reactivate(tourId, request).Result;
        actionResult.ShouldBeOfType<OkObjectResult>();
        var result = (actionResult as OkObjectResult)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Published
        result.ArchivedAt.ShouldBeNull();
        result.PublishedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Reactivate_Draft_Tour_Returns_BadRequest()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var tourId = -6; // RAZLIČITA Draft tura ZA REACTIVATE FAILURE TEST
        var request = new ReactivateRequestDto { NewStatus = 1 };

        // Act
        var result = controller.Reactivate(tourId, request).Result;

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest.ShouldNotBeNull();
        badRequest.Value.ShouldBe("Only archived tours can be reactivated.");
    }

    private static TourAuthoringController CreateController(IServiceScope scope)
    {
        return new TourAuthoringController(
            scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}