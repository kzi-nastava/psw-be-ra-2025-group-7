using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration
{
    [Collection("Sequential")]
    public class TourProblemCommandTests : BaseToursIntegrationTest
    {
        public TourProblemCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Create()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var dto = new TourProblemDto
            {
               
                TourId = 1,
                Category = "Equipment",
                Priority = "Medium",
                Description = "Lost item",
                TimeReported = DateTime.UtcNow
            };

            // Act
            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as TourProblemDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBeGreaterThan(0);
            result.TouristId.ShouldBe(-1); // jer faktički si ulogovan kao -1

            // Assert - Database
            var storedEntity = dbContext.TourProblems.Find(result.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.TourId.ShouldBe(dto.TourId);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new TourProblemDto
            {
                Description = "Test"
            };

            Should.Throw<ArgumentException>(() => controller.Create(dto));
        }

        [Fact]
        public void Updates()
        {
            // Arrange – seed entity -1 pripada turistu -1
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var dto = new TourProblemDto
            {
                Id = -1,
                TourId = 2,
                Category = "Other",
                Priority = "High",
                Description = "Updated description",
                TimeReported = DateTime.UtcNow
            };

            // Act
            var result = ((ObjectResult)controller.Update(dto.Id, dto).Result)?.Value as TourProblemDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(dto.Id);
            result.Description.ShouldBe(dto.Description);

            // Database check
            var stored = dbContext.TourProblems.Find(dto.Id);
            stored.ShouldNotBeNull();
            stored.Description.ShouldBe(dto.Description);
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new TourProblemDto
            {
                Id = -99999,
                Description = "Test"
            };

            Should.Throw<NotFoundException>(() => controller.Update(dto.Id, dto));
        }

        [Fact]
        public void Deletes()
        {
            // Arrange seed: entity -3 pripada turistu -1
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = (OkResult)controller.Delete(-3);

            // Assert response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert DB
            var stored = dbContext.TourProblems.FirstOrDefault(i => i.Id == -3);
            stored.ShouldBeNull();
        }

        private static TourProblemController CreateController(IServiceScope scope)
        {
            return new TourProblemController(
                scope.ServiceProvider.GetRequiredService<ITourProblemService>())
            {
                ControllerContext = BuildContext("-1") // Ulogovan turist = -1
            };
        }
    }
}
