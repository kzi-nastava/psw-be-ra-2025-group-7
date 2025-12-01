using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class KeyPointCommandTests : BaseToursIntegrationTest
    {
        public KeyPointCommandTests(ToursTestFactory factory) : base(factory) { }

        private static ITourService CreateService(IServiceScope scope)
        {
            return scope.ServiceProvider.GetRequiredService<ITourService>();
        }

        /// <summary>
        /// Pozitivan scenario:
        /// Autor kreira turu u Draft statusu i uspešno dodaje ključnu tačku.
        /// </summary>
        [Fact]
        public void Adds_key_point_to_draft_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = CreateService(scope);

            var tour = service.Create(new TourDto
            {
                AuthorId = 100,  // nebitno, Create ne proverava postojanje autora
                Name = "Test tour for key points",
                Description = "Integration test tour",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Price = 0
            });

            var keyPointDto = new KeyPointDto
            {
                Latitude = 44.8,
                Longitude = 20.47,
                Name = "Test key point",
                Description = "First key point",
                ImageUrl = null,
                Secret = "Secret text"
            };

            // Act
            var updated = service.AddKeyPoint(tour.Id, tour.AuthorId, keyPointDto);

            // Assert
            updated.KeyPoints.ShouldNotBeNull();
            updated.KeyPoints.Count.ShouldBe(1);

            var kp = updated.KeyPoints[0];
            kp.Name.ShouldBe(keyPointDto.Name);
            kp.Description.ShouldBe(keyPointDto.Description);
            kp.Latitude.ShouldBe(keyPointDto.Latitude);
            kp.Longitude.ShouldBe(keyPointDto.Longitude);
            kp.ImageUrl.ShouldBe(keyPointDto.ImageUrl);
            kp.Secret.ShouldBe(keyPointDto.Secret);
        }

        /// <summary>
        /// Pozitivan scenario:
        /// Autor može da izmeni postojeću ključnu tačku.
        /// </summary>
        [Fact]
        public void Updates_existing_key_point()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = CreateService(scope);

            var tour = service.Create(new TourDto
            {
                AuthorId = 101,
                Name = "Tour for update key point",
                Description = "Integration test tour - update",
                Difficulty = 1,
                Tags = new List<string> { "test-update" },
                Price = 0
            });

            var initialKeyPoint = new KeyPointDto
            {
                Latitude = 44.7866,
                Longitude = 20.4489,
                Name = "Initial point",
                Description = "Initial description",
                ImageUrl = null,
                Secret = "Initial secret"
            };

            var withKeyPoint = service.AddKeyPoint(tour.Id, tour.AuthorId, initialKeyPoint);

            var updatedKeyPoint = new KeyPointDto
            {
                Latitude = 44.8,
                Longitude = 20.47,
                Name = "Updated point",
                Description = "Updated description",
                ImageUrl = "https://example.com/updated.jpg",
                Secret = "Updated secret"
            };

            // Act
            var updatedTour = service.UpdateKeyPoint(withKeyPoint.Id, withKeyPoint.AuthorId, 0, updatedKeyPoint);

            // Assert
            updatedTour.KeyPoints.Count.ShouldBe(1);
            var kp = updatedTour.KeyPoints[0];

            kp.Name.ShouldBe(updatedKeyPoint.Name);
            kp.Description.ShouldBe(updatedKeyPoint.Description);
            kp.Latitude.ShouldBe(updatedKeyPoint.Latitude);
            kp.Longitude.ShouldBe(updatedKeyPoint.Longitude);
            kp.ImageUrl.ShouldBe(updatedKeyPoint.ImageUrl);
            kp.Secret.ShouldBe(updatedKeyPoint.Secret);
        }

        /// <summary>
        /// Pozitivan scenario:
        /// Autor može da obriše ključnu tačku sa ture u pripremi.
        /// </summary>
        [Fact]
        public void Removes_key_point_from_draft_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = CreateService(scope);

            var tour = service.Create(new TourDto
            {
                AuthorId = 102,
                Name = "Tour for remove key point",
                Description = "Integration test tour - remove",
                Difficulty = 1,
                Tags = new List<string> { "test-remove" },
                Price = 0
            });

            var keyPointDto = new KeyPointDto
            {
                Latitude = 44.7866,
                Longitude = 20.4489,
                Name = "Point to remove",
                Description = "To be removed",
                ImageUrl = null,
                Secret = "Secret"
            };

            var withKeyPoint = service.AddKeyPoint(tour.Id, tour.AuthorId, keyPointDto);
            withKeyPoint.KeyPoints.Count.ShouldBe(1); // sanity check

            // Act
            var updated = service.RemoveKeyPoint(withKeyPoint.Id, withKeyPoint.AuthorId, 0);

            // Assert
            updated.KeyPoints.ShouldNotBeNull();
            updated.KeyPoints.Count.ShouldBe(0);
        }

        /// <summary>
        /// Negativan scenario:
        /// Drugi autor ne sme da menja ključne tačke tuđe ture.
        /// </summary>
        [Fact]
        public void Adding_key_point_as_different_author_is_forbidden()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = CreateService(scope);

            var tour = service.Create(new TourDto
            {
                AuthorId = 200,
                Name = "Author's tour",
                Description = "Tour owned by author 200",
                Difficulty = 1,
                Tags = new List<string> { "forbidden-test" },
                Price = 0
            });

            var keyPointDto = new KeyPointDto
            {
                Latitude = 44.7866,
                Longitude = 20.4489,
                Name = "Forbidden point",
                Description = "Should not be added",
                ImageUrl = null,
                Secret = "Secret"
            };

            var otherAuthorId = tour.AuthorId + 1;

            // Act & Assert
            Should.Throw<ForbiddenException>(() =>
            {
                service.AddKeyPoint(tour.Id, otherAuthorId, keyPointDto);
            });
        }

        // Napomena:
        // Test za slučaj kada tura NIJE u Draft statusu (Published/Archived)
        // ostavljamo za člana tima koji implementira životni ciklus ture (Kartica 1),
        // jer će tek tada postojati način da se status legalno promeni iz domena.
    }
}
