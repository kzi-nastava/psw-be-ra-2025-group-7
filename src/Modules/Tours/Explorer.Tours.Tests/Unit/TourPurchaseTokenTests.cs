using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class TourPurchaseTokenTests
    {
        [Fact]
        public void Creates_token_with_valid_data()
        {
            // Arrange
            long userId = 1;
            long tourId = 10;

            // Act
            var token = new TourPurchaseToken(userId, tourId);

            // Assert
            token.UserId.ShouldBe(userId);
            token.TourId.ShouldBe(tourId);
            token.PurchaseDate.ShouldNotBe(default(DateTime));
            token.PurchaseDate.ShouldBeInRange(
                DateTime.UtcNow.AddSeconds(-5), 
                DateTime.UtcNow.AddSeconds(5));
        }

        [Fact]
        public void Creates_token_for_published_tour_using_factory()
        {
            // Arrange
            long userId = 1;
            var tour = CreatePublishedTour();

            // Act
            var token = TourPurchaseToken.CreateForTour(userId, tour);

            // Assert
            token.ShouldNotBeNull();
            token.UserId.ShouldBe(userId);
            token.TourId.ShouldBe(tour.Id);
            token.PurchaseDate.ShouldBeInRange(
                DateTime.UtcNow.AddSeconds(-5), 
                DateTime.UtcNow.AddSeconds(5));
        }

        [Fact]
        public void Factory_throws_exception_for_null_tour()
        {
            // Arrange
            long userId = 1;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => 
                TourPurchaseToken.CreateForTour(userId, null!));
        }

        [Fact]
        public void Factory_throws_exception_for_draft_tour()
        {
            // Arrange
            long userId = 1;
            var tour = new Tour(1, "Draft Tour", "Description", 
                TourDifficulty.Easy, new List<string> { "tag1" });
            // Tour is in Draft status by default

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => 
                TourPurchaseToken.CreateForTour(userId, tour))
                .Message.ShouldContain("Only published tours can be purchased");
        }

        [Fact]
        public void Factory_throws_exception_for_archived_tour()
        {
            // Arrange
            long userId = 1;
            var tour = CreatePublishedTour();
            tour.Archive(); // Archive the tour

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => 
                TourPurchaseToken.CreateForTour(userId, tour))
                .Message.ShouldContain("Archived tours cannot be purchased");
        }

        [Fact]
        public void Token_preserves_purchase_timestamp()
        {
            // Arrange
            long userId = 1;
            long tourId = 10;
            var beforeCreation = DateTime.UtcNow;

            // Act
            var token = new TourPurchaseToken(userId, tourId);
            var afterCreation = DateTime.UtcNow;

            // Assert
            token.PurchaseDate.ShouldBeGreaterThanOrEqualTo(beforeCreation);
            token.PurchaseDate.ShouldBeLessThanOrEqualTo(afterCreation);
        }

        [Fact]
        public void Multiple_tokens_have_unique_purchase_dates()
        {
            // Arrange
            long userId = 1;
            var tour1 = CreatePublishedTour(1);
            var tour2 = CreatePublishedTour(2);

            // Act
            var token1 = TourPurchaseToken.CreateForTour(userId, tour1);
            System.Threading.Thread.Sleep(10); // Small delay
            var token2 = TourPurchaseToken.CreateForTour(userId, tour2);

            // Assert
            token1.PurchaseDate.ShouldNotBe(token2.PurchaseDate);
            token2.PurchaseDate.ShouldBeGreaterThan(token1.PurchaseDate);
        }

        [Fact]
        public void Token_can_hold_tour_reference()
        {
            // Arrange
            long userId = 1;
            var tour = CreatePublishedTour();

            // Act
            var token = TourPurchaseToken.CreateForTour(userId, tour);

            // Assert - Tour property is init-only, can be set via reflection or constructor
            token.TourId.ShouldBe(tour.Id);
        }

        // Helper method to create a published tour for testing
        private static Tour CreatePublishedTour(long id = 1)
        {
            var tour = new Tour(1, $"Test Tour {id}", "Test Description", 
                TourDifficulty.Medium, new List<string> { "test", "published" });
            
            // Add required data for publishing
            var keyPoint1 = new KeyPoint(45.0, 19.0, "Start", "Starting point", "Secret 1");
            var keyPoint2 = new KeyPoint(45.1, 19.1, "End", "Ending point", "Secret 2");
            tour.AddKeyPoint(keyPoint1);
            tour.AddKeyPoint(keyPoint2);
            
            var duration = new TourDuration(TravelType.Walk, 60);
            tour.AddDuration(duration);
            
            tour.Publish();
            
            // Use reflection to set Id since it's an Entity property
            var idProperty = typeof(Tour).BaseType?.GetProperty("Id");
            idProperty?.SetValue(tour, id);
            
            return tour;
        }
    }
}
