using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class TourPreferencesQueryTests : BaseStakeholdersIntegrationTest
    {
        public TourPreferencesQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Get_returns_preferences_for_authenticated_tourist()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Očistimo test podatke i kreiramo novi entitet
            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var testDto = new TourPreferencesDto
            {
                TouristId = -21, // ID koji ćemo postaviti u claim
                PreferredDifficulty = 2,
                WalkingRating = 1,
                BicycleRating = 2,
                CarRating = 3,
                BoatRating = 0,
                Tags = new List<string> { "nature", "history" }
            };

            var controller = CreateController(scope, "-21");
            controller.Create(testDto); // kreiramo preference

            // Act
            var actionResult = controller.Get();
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as TourPreferencesDto;

            // Assert
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(-21);
            result.PreferredDifficulty.ShouldBe(testDto.PreferredDifficulty);
            result.WalkingRating.ShouldBe(testDto.WalkingRating);
            result.BicycleRating.ShouldBe(testDto.BicycleRating);
            result.CarRating.ShouldBe(testDto.CarRating);
            result.BoatRating.ShouldBe(testDto.BoatRating);
            result.Tags.Count.ShouldBe(2);
            result.Tags.ShouldContain("nature");
            result.Tags.ShouldContain("history");
        }

        [Fact]
        public void Get_returns_NotFound_when_no_preferences()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            dbContext.TourPreferences.RemoveRange(dbContext.TourPreferences);
            dbContext.SaveChanges();

            var controller = CreateController(scope, "-99"); // turista koji nema preference

            // Act
            var actionResult = controller.Get();
            var notFoundResult = actionResult.Result as NotFoundObjectResult;

            // Assert
            notFoundResult.ShouldNotBeNull();
            notFoundResult.StatusCode.ShouldBe(404);
            notFoundResult.Value.ShouldBe("You have zero preferences.");
        }

        private static TourPreferencesController CreateController(IServiceScope scope, string touristId)
        {
            var controller = new TourPreferencesController(
                scope.ServiceProvider.GetRequiredService<ITourPreferencesService>());

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, touristId),
                    new Claim("id", touristId),
                    new Claim(ClaimTypes.Role, "tourist")
                }, "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            return controller;
        }
    }
}