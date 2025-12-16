using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new CreateTourDto
        {
            Name = "New Test Tour",
            Description = "Test description for new tour",
            Difficulty = 1,
            Tags = new List<string> { "test", "new" },
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Status.ShouldBe(0); // Draft
        result.Price.ShouldBe(0);
        result.PublishedAt.ShouldBeNull();
        result.ArchivedAt.ShouldBeNull();

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Status.ShouldBe(TourStatus.Draft);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var newEntity = new CreateTourDto
        {
            Name = "",  
            Description = "Test",
            Difficulty = 1,
            Tags = new List<string>(),
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(newEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new TourDto
        {
            Id = -1,
            Name = "Updated Test Tour",
            Description = "Updated description",
            Difficulty = 2,
            Tags = new List<string> { "updated" },
            Status = 0,
            Price = 99,
            AuthorId = -1
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity.Id, updatedEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Difficulty.ShouldBe(2);
        result.Price.ShouldBe(99);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Name == "Updated Test Tour");
        storedEntity.ShouldNotBeNull();
        storedEntity.Difficulty.ShouldBe(TourDifficulty.Hard);
        var oldEntity = dbContext.Tours.FirstOrDefault(t => t.Name == "Test Tour 1");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new TourDto
        {
            Id = -1000,
            Name = "Test",
            Description = "Test",
            Difficulty = 1,
            Tags = new List<string>(),
            Status = 0,
            Price = 0,
            AuthorId = -1
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity.Id, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-2);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Id == -2);
        storedEntity.ShouldBeNull();
    }
    [Fact]
    public void AddKeyPoint_with_make_public_creates_public_point_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // prvo kreiramo novu turu za ovog autora (-1), da smo sigurni da je Draft
        var newTour = new CreateTourDto
        {
            Name = "Tour with public keypoint",
            Description = "Desc",
            Difficulty = 1,
            Tags = new List<string> { "public-test" }
        };

        var createdResult = ((ObjectResult)controller.Create(newTour).Result)?.Value as TourDto;
        createdResult.ShouldNotBeNull();
        var tourId = createdResult!.Id;

        var keyPointDto = new KeyPointDto
        {
            Latitude = 45.20,
            Longitude = 19.80,
            Name = "Public KP",
            Description = "Some description",
            ImageUrl = null,
            Secret = "Secret data",
            MakePublic = true      // 🔥 bitno!
        };

        // Act
        var addResult = ((ObjectResult)controller.AddKeyPoint(tourId, keyPointDto).Result);

        // Assert - response
        addResult.ShouldNotBeNull();
        addResult.StatusCode.ShouldBe(200);

        // Assert - PublicPointRequest u bazi
        var request = dbContext.PublicPointRequests
            .FirstOrDefault(r => r.TourId == tourId && r.AuthorId == -1);

        request.ShouldNotBeNull();
        request!.Status.ShouldBe(PublicPointRequestStatus.Pending);
        request.KeyPointIndex.ShouldBe(0); // prva (i jedina) tačka na toj turi
    }


    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    [Fact]
    public void Delete_fails_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Delete(-3))
            .Message.ShouldBe("Only Draft tours can be deleted.");
    }

    private static TourAuthoringController CreateController(IServiceScope scope)
    {
        return new TourAuthoringController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
