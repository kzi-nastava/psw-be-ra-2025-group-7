using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Tests.Integration.Clubs
{
    [Collection("Sequential")]
    public class ClubQueryTests : BaseStakeholdersIntegrationTest
    {
        public ClubQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var newEntity = new ClubDto
            {
                Name = "Klub za pretragu",
                Description = "Opis za Retrieves_all",
                CreatedBy = 1,
                ImageUrls = new List<string> { "retrieves-all.jpg" }
            };

            // ---- Act 1: Create ----
            var createdResult = controller.Create(newEntity);
            var created = (createdResult.Result as OkObjectResult)?.Value as ClubDto;

            // ---- Assert – Response (CREATE) ----
            created.ShouldNotBeNull();
            created.Id.ShouldNotBe(0);
            created.Name.ShouldBe(newEntity.Name);
            created.Description.ShouldBe(newEntity.Description);
            created.CreatedBy.ShouldBe(newEntity.CreatedBy);
            created.ImageUrls.ShouldBeEquivalentTo(newEntity.ImageUrls);
            created.CreatedAt.ShouldNotBe(default);
            created.UpdatedAt.ShouldNotBe(default);
            created.UpdatedAt.ShouldBeGreaterThanOrEqualTo(created.CreatedAt);

            // ---- Assert – Database (CREATE) ----
            var stored = dbContext.Clubs.First(c => c.Id == created.Id);

            stored.ShouldNotBeNull();
            stored.Name.ShouldBe(newEntity.Name);
            stored.Description.ShouldBe(newEntity.Description);
            stored.CreatedBy.ShouldBe(newEntity.CreatedBy);
            stored.ImageUrls.ShouldBeEquivalentTo(newEntity.ImageUrls);
            stored.CreatedAt.ShouldNotBe(default);
            stored.UpdatedAt.ShouldNotBe(default);
            stored.UpdatedAt.ShouldBeGreaterThanOrEqualTo(stored.CreatedAt);

            // ---- Act 2: GetAll ----
            var actionResult = controller.GetAll();
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as List<ClubDto>;

            // ---- Assert – GET ALL ----
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.Any(c => c.Id == created.Id && c.Name == created.Name).ShouldBeTrue();
        }

        private static ClubsController CreateController(IServiceScope scope)
        {
            return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
            {
                ControllerContext = BuildContext("1")
            };
        }
    }
}
