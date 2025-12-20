using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Tourist;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Encounters.Tests.Integration
{
    [Collection("Sequential")]
    public class EncounterQueryTests : BaseEncountersIntegrationTest
    {
        public EncounterQueryTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Admin_can_filter_by_status_and_type()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope, "1");
            
            //Arrange
            var e1 = CreateEncounter(controller, "Social Active", "social");
            var e2 = CreateEncounter(controller, "Location Active", "location");
            var e3 = CreateEncounter(controller, "Misc Draft", "misc");

            controller.ChangeStatus(e1.Id, new ChangeEncounterStatusDto { Status = "active" });
            controller.ChangeStatus(e2.Id, new ChangeEncounterStatusDto { Status = "active" });

            //Act
            var result = controller.Get(status: "active", type: "social");
            var okResult = result.Result as OkObjectResult;
            var list = okResult?.Value as IEnumerable<EncounterDto>;

            //Assert
            list.ShouldNotBeNull();
            list.Count().ShouldBe(1);
            list.First().Id.ShouldBe(e1.Id);
            list.First().Type.ShouldBe("social");
            list.First().Status.ShouldBe("active");
        }

        [Fact]
        public void Tourist_gets_only_active_encounters()
        {
            using var scope = Factory.Services.CreateScope();

            // Arrange
            var adminController = CreateAdminController(scope, "1");

            var e1 = CreateEncounter(adminController, "Active Social", "social");
            var e2 = CreateEncounter(adminController, "Active Location", "location");
            var e3 = CreateEncounter(adminController, "Draft Misc", "misc");

            adminController.ChangeStatus(e1.Id, new ChangeEncounterStatusDto { Status = "active" });
            adminController.ChangeStatus(e2.Id, new ChangeEncounterStatusDto { Status = "active" });

            // Act
            var touristController = CreateTouristController(scope, "2");
            var result = touristController.GetActiveEncounters();
            var okResult = result.Result as OkObjectResult;
            var encounters = okResult?.Value as IEnumerable<EncounterDto>;

            // Assert
            encounters.ShouldNotBeNull();
            encounters.Count().ShouldBe(2);
            encounters.All(e => e.Status == "active").ShouldBeTrue();
            encounters.Any(e => e.Id == e1.Id).ShouldBeTrue();
            encounters.Any(e => e.Id == e2.Id).ShouldBeTrue();
            encounters.Any(e => e.Id == e3.Id).ShouldBeFalse(); 
        }

        [Fact]
        public void Tourist_gets_empty_list_when_no_active_encounters()
        {
            using var scope = Factory.Services.CreateScope();

            // Arrange
            var adminController = CreateAdminController(scope, "1");
            CreateEncounter(adminController, "Draft 1", "social");
            CreateEncounter(adminController, "Draft 2", "location");

            // Act
            var touristController = CreateTouristController(scope, "2");
            var result = touristController.GetActiveEncounters();
            var okResult = result.Result as OkObjectResult;
            var encounters = okResult?.Value as IEnumerable<EncounterDto>;

            // Assert
            encounters.ShouldNotBeNull();
            encounters.Count().ShouldBe(0);
        }

        private static EncounterDto CreateEncounter(EncountersController controller, string name, string type)
        {
            var dto = new CreateEncounterDto
            {
                Name = name,
                Description = "Test description",
                Latitude = 45.2671,
                Longitude = 19.8335,
                Xp = 10,
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

        private static TouristEncountersController CreateTouristController(IServiceScope scope, string userId)
        {
            var controller = new TouristEncountersController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>());

            var ctx = BuildContext(userId);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "tourist")
            }, "test");

            ctx.HttpContext.User = new ClaimsPrincipal(identity);
            controller.ControllerContext = ctx;
            return controller;
        }
    }
}