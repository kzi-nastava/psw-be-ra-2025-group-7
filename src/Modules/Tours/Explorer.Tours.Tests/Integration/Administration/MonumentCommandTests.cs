using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentCommandTests : BaseToursIntegrationTest
{
    public MonumentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new MonumentDto
        {
            Name = "?avolja varoš",
            Description = "Prirodni fenomen sa kamenim formacijama",
            YearOfCreation = 1995,
            Latitude = 42.9667,
            Longitude = 21.9333
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Status.ShouldBe(0); // Active

        // Assert - Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new MonumentDto
        {
            Name = "",
            Description = "Test",
            YearOfCreation = 2000,
            Latitude = 44.0,
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_invalid_latitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new MonumentDto
        {
            Name = "Test Monument",
            Description = "Test Description",
            YearOfCreation = 2000,
            Latitude = 100.0, // Invalid - mora biti izme?u -90 i 90
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new MonumentDto
        {
            Id = -1,
            Name = "Beogradska tvr?ava",
            Description = "Ažurirani opis tvr?ave na uš?u Save u Dunav",
            YearOfCreation = 1403,
            Status = 0,
            Latitude = 44.8225,
            Longitude = 20.4508
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Name == "Beogradska tvr?ava");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        var oldEntity = dbContext.Monuments.FirstOrDefault(i => i.Name == "Kalemegdanska tvr?ava");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Id = -1000,
            Name = "Test Monument",
            Description = "Test",
            YearOfCreation = 2000,
            Status = 0,
            Latitude = 44.0,
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Monuments.FirstOrDefault(i => i.Id == -3);
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

    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
