using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public.Tourist;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class LocationServiceTests : BaseStakeholdersIntegrationTest
    {
        public LocationServiceTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_nearby_monuments_for_tourist_with_location()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21"); // Tourist with location (Belgrade center)
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Act
            var actionResult = controller.GetNearbyMonuments();
            var result = ((ObjectResult)actionResult.Result)?.Value as List<MonumentDto>;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3); // All 3 monuments from test data
            
            // Verify monuments are ordered by distance (closest first)
            // Tourist is at Belgrade center (44.8176, 20.4633)
            // Kalemegdanska tvrđava should be first (44.8225, 20.4508) - very close
            result[0].Name.ShouldBe("Kalemegdanska tvr?ava");
            result[0].Latitude.ShouldBe(44.8225);
            result[0].Longitude.ShouldBe(20.4508);
            
            // All monuments should have required properties
            foreach (var monument in result)
            {
                monument.Id.ShouldNotBe(0);
                monument.Name.ShouldNotBeNullOrEmpty();
                monument.Description.ShouldNotBeNullOrEmpty();
                monument.YearOfCreation.ShouldBeGreaterThan(0);
                monument.Latitude.ShouldBeInRange(-90, 90);
                monument.Longitude.ShouldBeInRange(-180, 180);
            }
        }

        [Fact]
        public void Gets_nearby_monuments_limits_to_40()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");

            // Act
            var actionResult = controller.GetNearbyMonuments();
            var result = ((ObjectResult)actionResult.Result)?.Value as List<MonumentDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeLessThanOrEqualTo(40); // Should respect the limit
        }

        [Fact]
        public void Fails_to_get_nearby_monuments_for_tourist_without_location()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22"); // Tourist without location

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.GetNearbyMonuments());
        }

        [Fact]
        public void Fails_to_get_nearby_monuments_for_nonexistent_user()
        {

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var existingUserId = dbContext.Users.Select(u => u.Id).First();

            var profile = dbContext.UserProfiles.SingleOrDefault(p => p.UserId == existingUserId);
            if (profile != null)
            {
                dbContext.UserProfiles.Remove(profile);
                dbContext.SaveChanges();
            }

            var controller = CreateController(scope, existingUserId.ToString());
            Should.Throw<ArgumentException>(() => controller.GetNearbyMonuments());

        }

        [Fact]
        public void Updates_location_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var newLocation = new TouristLocationDto
            {
                Latitude = 45.2551,
                Longitude = 19.8636
            };

            // Act
            var result = ((ObjectResult)controller.Update(newLocation))?.Value as TouristLocationDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Latitude.ShouldBe(45.2551);
            result.Longitude.ShouldBe(19.8636);

            // Assert - Database
            dbContext.ChangeTracker.Clear();
            var storedProfile = dbContext.UserProfiles.FirstOrDefault(up => up.UserId == -21);
            storedProfile.ShouldNotBeNull();
            storedProfile.CurrentLatitude.ShouldBe(45.2551);
            storedProfile.CurrentLongitude.ShouldBe(19.8636);
        }

        [Fact]
        public void Gets_location_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");

            // Act
            var actionResult = controller.Get();
            var result = ((ObjectResult)actionResult.Result)?.Value as TouristLocationDto;

            // Assert
            result.ShouldNotBeNull();
            result.Latitude.ShouldBe(44.8176);
            result.Longitude.ShouldBe(20.4633);
        }

        private static LocationController CreateController(IServiceScope scope, string userId)
        {
            return new LocationController(
                scope.ServiceProvider.GetRequiredService<ILocationService>())
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim("id", userId),
                            new Claim("personId", userId)
                        }))
                    }
                }
            };
        }
    }
}
