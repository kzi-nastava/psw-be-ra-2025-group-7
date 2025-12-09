using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
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
                TimeReported = DateTime.UtcNow,
                IsSolved = false
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
            int id = -1;

            // ===== FIRST SCOPE: ensure entity exists (via controller.Create) =====
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

                var exists = dbContext.TourProblems.FirstOrDefault(i => i.Id == id);

                if (exists == null)
                {
                    var seedDto = new TourProblemDto
                    {
                        TourId = 1,
                        Category = "Equipment",
                        Priority = "Medium",
                        Description = "Initial equipment issue to be updated.",
                        TimeReported = DateTime.UtcNow,
                        IsSolved = false
                    };

                    var created = ((ObjectResult)controller.Create(seedDto).Result)?.Value as TourProblemDto;
                    created.ShouldNotBeNull();
                    id = created.Id;    // ako Create napravi nov
                }
            }

            // ===== SECOND SCOPE: perform update =====
            TourProblemDto updated;

            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);

                var dto = new TourProblemDto
                {
                    Id = id,
                    TourId = 2,
                    Category = "Other",
                    Priority = "High",
                    Description = "Updated description",
                    TimeReported = DateTime.UtcNow,
                    IsSolved = false
                };

                updated = ((ObjectResult)controller.Update(id, dto).Result)?.Value as TourProblemDto;
                updated.ShouldNotBeNull();
                updated.Description.ShouldBe(dto.Description);
            }

            // ===== THIRD SCOPE: verify =====
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var stored = dbContext.TourProblems.Find(id);

                stored.ShouldNotBeNull();
                stored.Description.ShouldBe("Updated description");
            }
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
            int idToDelete = -3;

            // ===== FIRST SCOPE (check or create record) =====
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var controller = CreateController(scope);

                var existing = dbContext.TourProblems.FirstOrDefault(i => i.Id == idToDelete);

                if (existing == null)
                {
                    var dto = new TourProblemDto
                    {
                        TourId = 2,
                        Category = "Safety",
                        Priority = "Low",
                        Description = "Safety related problem to be deleted.",
                        TimeReported = DateTime.UtcNow,
                        IsSolved = false
                    };

                    var created = ((ObjectResult)controller.Create(dto).Result)?.Value as TourProblemDto;
                    created.ShouldNotBeNull();
                    idToDelete = created.Id;
                }
            }

            // ===== SECOND SCOPE (actual delete) =====
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

                // Act
                var result = (OkResult)controller.Delete(idToDelete);

                // Assert response
                result.ShouldNotBeNull();
                result.StatusCode.ShouldBe(200);
            }

            // ===== THIRD SCOPE (DB assertion) =====
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
                var deleted = dbContext.TourProblems.FirstOrDefault(i => i.Id == idToDelete);
                deleted.ShouldBeNull();
            }
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
