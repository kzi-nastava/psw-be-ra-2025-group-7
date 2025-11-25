using Explorer.API.Controllers.Tourist.Tours;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourJournalCommandTests : BaseToursIntegrationTest
{
    public TourJournalCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new TourJournalDto
        {
            Name = "Moja avantura u Londonu",
            Country = "Velika Britanija",
            City = "London"
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourJournalDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.TouristId.ShouldBe(-21);
        result.Status.ShouldBe("Draft");
        
        // Assert - Database
        var storedEntity = dbContext.TourJournals.FirstOrDefault(tj => tj.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.TouristId.ShouldBe(-21);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new TourJournalDto
        {
            Name = "",
            Country = "Srbija"
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
        var updatedEntity = new TourJournalDto
        {
            Id = -1,
            Name = "Ažurirani dnevnik",
            Country = "Srbija",
            City = "Novi Sad"
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourJournalDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.City.ShouldBe(updatedEntity.City);

        // Assert - Database
        var storedEntity = dbContext.TourJournals.FirstOrDefault(tj => tj.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
        storedEntity.City.ShouldBe(updatedEntity.City);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new TourJournalDto
        {
            Id = -1000,
            Name = "Test",
            Country = "Test"
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Update_fails_not_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerForDifferentUser(scope);
        var updatedEntity = new TourJournalDto
        {
            Id = -1,
            Name = "Pokušaj ažuriranja tuðeg dnevnika",
            Country = "Srbija"
        };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Update(updatedEntity));
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
        var storedEntity = dbContext.TourJournals.FirstOrDefault(tj => tj.Id == -2);
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
    public void Delete_fails_not_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerForDifferentUser(scope);

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Delete(-1));
    }
    
    private static TourJournalController CreateController(IServiceScope scope)
    {
        return new TourJournalController(scope.ServiceProvider.GetRequiredService<ITourJournalService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }

    private static TourJournalController CreateControllerForDifferentUser(IServiceScope scope)
    {
        return new TourJournalController(scope.ServiceProvider.GetRequiredService<ITourJournalService>())
        {
            ControllerContext = BuildContext("-99")
        };
    }
}
