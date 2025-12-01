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
        var newEntity = new TourDto
        {
            Name = "New Test Tour",
            Description = "Test description for new tour",
            Difficulty = 1,
            Tags = new List<string> { "test", "new" },
            Status = 0,
            Price = 0,
            AuthorId = -1
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var newEntity = new TourDto
        {
            Name = "",  // Invalid - empty name
            Description = "Test"
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
