using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
public class AnnualAwardCommandTests : BaseToursIntegrationTest
{
    public AnnualAwardCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new CreateAnnualAwardDto
        {
            Name = "Excellence Award 2025",
            Description = "Award for excellence in tour design and execution",
            Year = 2025,
            VotingStartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),  
            VotingEndDate = DateTime.SpecifyKind(new DateTime(2025, 12, 31, 23, 59, 59), DateTimeKind.Utc)  
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as AnnualAwardDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Description.ShouldBe(newEntity.Description);
        result.Year.ShouldBe(newEntity.Year);
        result.Status.ShouldBe("Draft");

        // Assert - Database
        var storedEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new CreateAnnualAwardDto
        {
            Name = "", // Invalid: empty name
            Description = "Test",
            Year = 2025,
            VotingStartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
            VotingEndDate = DateTime.SpecifyKind(new DateTime(2025, 12, 31, 23, 59, 59), DateTimeKind.Utc)
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
        var updatedEntity = new UpdateAnnualAwardDto
        {
            Id = -1,
            Name = "Best Tour 2024 - Updated",
            Description = "Updated description for the award",
            Year = 2024,
            VotingStartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
            VotingEndDate = DateTime.SpecifyKind(new DateTime(2025, 12, 31, 23, 59, 59), DateTimeKind.Utc)
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as AnnualAwardDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Name == "Best Tour 2024 - Updated");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        var oldEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Name == "Best Tour 2024");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new UpdateAnnualAwardDto
        {
            Id = -1000,
            Name = "Test",
            Description = "Test",
            Year = 2025,
            VotingStartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
            VotingEndDate = DateTime.SpecifyKind(new DateTime(2025, 12, 31, 23, 59, 59), DateTimeKind.Utc)
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (NoContentResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        // Assert - Database
        var storedEntity = dbContext.AnnualAwards.FirstOrDefault(i => i.Id == -3);
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

    private static AnnualAwardController CreateController(IServiceScope scope)
    {
        return new AnnualAwardController(scope.ServiceProvider.GetRequiredService<IAnnualAwardService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}