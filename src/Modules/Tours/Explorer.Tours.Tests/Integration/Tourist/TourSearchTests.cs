using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist.Tours;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tours
{
    public class TourSearchTests : IClassFixture<ToursTestFactory>
    {
        private readonly ToursTestFactory _factory;

        public TourSearchTests(ToursTestFactory factory)
        {
            _factory = factory;
        }

        private static TourSearchController CreateController(IServiceScope scope)
        {
            return new TourSearchController(
                scope.ServiceProvider.GetRequiredService<ITourSearchService>());
        }


        [Theory]
        // oko KeyPointa published ture -3 (45.2551, 19.8636)
        [InlineData(45.2551, 19.8636, 5.0, 3)]
        // Daleka lokacija – ne bi trebalo nista da vrati
        [InlineData(0.0, 0.0, 1.0, 0)]
        public void SearchByLocation_returns_expected_number_of_tours(
            double lat,
            double lon,
            double radiusKm,
            int expectedCount)
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var actionResult = controller.SearchByLocation(lat, lon, radiusKm);
            var result = actionResult.Result as ObjectResult;

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var tours = result.Value as List<TourDto>;
            tours.ShouldNotBeNull();
            tours.Count.ShouldBe(expectedCount);

            // sve ture moraju biti Published
            if (tours.Any())
            {
                tours.All(t => t.Status == (int)TourStatus.Published).ShouldBeTrue();
            }
        }

        // Nevalidan radijus
        [Theory]
        [InlineData(45.2551, 19.8636, 0)]
        [InlineData(45.2551, 19.8636, -5)]
        public void SearchByLocation_fails_for_invalid_radius(
            double lat,
            double lon,
            double radiusKm)
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var actionResult = controller.SearchByLocation(lat, lon, radiusKm);
            var result = actionResult.Result as ObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }
    }
}