using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Integration
{
    [Collection("Sequential")]
    public class EncounterCommandTests : BaseEncountersIntegrationTest
    {
        public EncounterCommandTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_encounter_as_draft_by_default_and_persists()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var controller = CreateAdminController(scope, "1");

            var dto = new CreateEncounterDto
            {
                Name = "Test encounter",
                Description = "Do something",
                Latitude = 45.2671,
                Longitude = 19.8335,
                Xp = 10,
                Type = "social"
            };

            // Act
            var actionResult = controller.Create(dto);
            var okResult = actionResult.Result as OkObjectResult;
            var created = okResult?.Value as EncounterDto;

            // Assert - response
            created.ShouldNotBeNull();
            created!.Id.ShouldBeGreaterThan(0);
            created.Name.ShouldBe(dto.Name);
            created.Xp.ShouldBe(10);
            created.Status.ShouldBe("draft"); 
            created.Type.ShouldBe("social");

            // Assert - db
            var stored = dbContext.Encounters.FirstOrDefault(x => x.Id == created.Id);
            stored.ShouldNotBeNull();
            stored!.Name.ShouldBe(dto.Name);
            stored.Xp.ShouldBe(10);
            stored.Status.ToString().ToLower().ShouldBe("draft");
        }

        [Fact]
        public void Admin_can_activate_and_filter_active_social_encounters()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope, "1");

            // Arrange: kreiraj 2 encountera
            var e1 = CreateEncounter(controller, "E1", "social");
            var e2 = CreateEncounter(controller, "E2", "location");

            // Act: aktiviraj samo e1
            var statusResult = controller.ChangeStatus(e1.Id, new ChangeEncounterStatusDto { Status = "active" });
            var ok = statusResult.Result as OkObjectResult;
            var updated = ok?.Value as EncounterDto;

            updated.ShouldNotBeNull();
            updated!.Status.ShouldBe("active");

            // Query: filtriraj
            var queryResult = controller.Get(status: "active", type: "social");
            var ok2 = queryResult.Result as OkObjectResult;
            var list = ok2?.Value as IEnumerable<EncounterDto>;

            list.ShouldNotBeNull();
            list!.Count().ShouldBe(1);
            list.First().Id.ShouldBe(e1.Id);
        }

        private static EncounterDto CreateEncounter(EncountersController controller, string name, string type)
        {
            var dto = new CreateEncounterDto
            {
                Name = name,
                Description = "Desc",
                Latitude = 45.0,
                Longitude = 19.0,
                Xp = 5,
                Type = type
            };

            var actionResult = controller.Create(dto);
            var ok = actionResult.Result as OkObjectResult;
            var created = ok?.Value as EncounterDto;

            created.ShouldNotBeNull();
            return created!;
        }

        private static EncountersController CreateAdminController(IServiceScope scope, string userId)
        {
            var controller = new EncountersController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>());

            var ctx = BuildContext(userId);

            var identity = new ClaimsIdentity(new[]
            {
            new Claim("id", userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "administrator")
        }, "test");

            ctx.HttpContext.User = new ClaimsPrincipal(identity);
            controller.ControllerContext = ctx;
            return controller;
        }
    }

}