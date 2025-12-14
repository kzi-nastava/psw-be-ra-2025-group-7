using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration.Shopping
{
    [Collection("Sequential")]
    public class ShoppingCartPurchaseIntegrationTests : BaseToursIntegrationTest
    {
        public ShoppingCartPurchaseIntegrationTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Purchases_all_items_in_cart_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 200);
            CleanupPurchaseTokens(scope, 200);
            var controller = CreateController(scope, "200");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Add items to cart
            controller.AddToCart(new AddToCartDto { TourId = -10 });
            controller.AddToCart(new AddToCartDto { TourId = -11 });

            // Act
            var result = controller.PurchaseCart();
            var okResult = result.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - Response
            tokens.ShouldNotBeNull();
            tokens.Count.ShouldBe(2);
            tokens.All(t => t.UserId == 200).ShouldBeTrue();
            tokens.Any(t => t.TourId == -10).ShouldBeTrue();
            tokens.Any(t => t.TourId == -11).ShouldBeTrue();

            // Assert - Tokens have tour information
            tokens.All(t => t.Tour != null).ShouldBeTrue();
            foreach (var token in tokens)
            {
                token.Tour!.Id.ShouldBe(token.TourId);
                token.Tour.KeyPoints.ShouldNotBeNull();
                token.Tour.KeyPoints.Count.ShouldBeGreaterThan(0);
                // Verify secrets are not included
                token.Tour.KeyPoints.All(kp => kp.GetType().Name == "KeyPointWithoutSecretDto").ShouldBeTrue();
            }

            // Assert - Database: tokens created
            dbContext.ChangeTracker.Clear();
            var storedTokens = dbContext.TourPurchaseTokens
                .Where(t => t.UserId == 200)
                .ToList();
            storedTokens.Count.ShouldBe(2);

            // Assert - Database: cart cleared
            var storedCart = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == 200);
            storedCart.Items.ShouldBeEmpty();
        }

        [Fact]
        public void Purchases_single_item_from_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 201);
            CleanupPurchaseTokens(scope, 201);
            var controller = CreateController(scope, "201");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.AddToCart(new AddToCartDto { TourId = -10 });

            // Act
            var result = controller.PurchaseCart();
            var okResult = result.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert
            tokens.ShouldNotBeNull();
            tokens.Count.ShouldBe(1);
            tokens[0].TourId.ShouldBe(-10);
            tokens[0].UserId.ShouldBe(201);

            // Assert - Token includes tour info
            tokens[0].Tour.ShouldNotBeNull();
            tokens[0].Tour!.Name.ShouldNotBeNullOrEmpty();
            tokens[0].Tour.Description.ShouldNotBeNullOrEmpty();
            tokens[0].Tour.KeyPoints.ShouldNotBeEmpty();
        }

        [Fact]
        public void Purchase_fails_for_empty_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 202);
            CleanupPurchaseTokens(scope, 202);
            var controller = CreateController(scope, "202");

            // Act
            var result = controller.PurchaseCart();
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            badResult.ShouldNotBeNull();
            // PurchaseCart returns an anonymous object with a 'message' property
            var errorObj = badResult.Value;
            var messageProperty = errorObj?.GetType().GetProperty("message");
            string errorMessage = messageProperty?.GetValue(errorObj)?.ToString() ?? "";
            errorMessage.ShouldContain("Cannot purchase an empty cart");
        }

        [Fact]
        public void Purchase_fails_if_tour_already_purchased()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 203);
            CleanupPurchaseTokens(scope, 203);
            var controller = CreateController(scope, "203");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Add to cart and purchase first time
            controller.AddToCart(new AddToCartDto { TourId = -3 });
            controller.PurchaseCart();

            // Add same tour to cart again
            controller.AddToCart(new AddToCartDto { TourId = -3 });

            // Act - Try to purchase again
            var result = controller.PurchaseCart();
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            badResult.ShouldNotBeNull();
            // PurchaseCart returns an anonymous object with a 'message' property
            var errorObj = badResult.Value;
            var messageProperty = errorObj?.GetType().GetProperty("message");
            string errorMessage = messageProperty?.GetValue(errorObj)?.ToString() ?? "";
            errorMessage.ShouldContain("already been purchased");
        }

        [Fact]
        public void Purchase_clears_cart_after_successful_transaction()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 204);
            CleanupPurchaseTokens(scope, 204);
            var controller = CreateController(scope, "204");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.AddToCart(new AddToCartDto { TourId = -3 });
            controller.AddToCart(new AddToCartDto { TourId = -5 });

            // Act
            controller.PurchaseCart();

            // Assert - Cart is cleared
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == 204);
            cart.Items.ShouldBeEmpty();
        }

        [Fact]
        public void Purchase_creates_tokens_with_correct_timestamps()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 205);
            CleanupPurchaseTokens(scope, 205);
            var controller = CreateController(scope, "205");
            var beforePurchase = DateTime.UtcNow;

            controller.AddToCart(new AddToCartDto { TourId = -3 });

            // Act
            var result = controller.PurchaseCart();
            var afterPurchase = DateTime.UtcNow;
            var okResult = result.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert
            tokens.ShouldNotBeNull();
            tokens[0].PurchaseDate.ShouldBeGreaterThanOrEqualTo(beforePurchase);
            tokens[0].PurchaseDate.ShouldBeLessThanOrEqualTo(afterPurchase);
        }

        [Fact]
        public void Purchase_validates_all_tours_are_published()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 206);
            CleanupPurchaseTokens(scope, 206);
            var controller = CreateController(scope, "206");

            // Try to add archived tour (should fail in AddToCart)
            var result = controller.AddToCart(new AddToCartDto { TourId = -100 });
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            badResult.ShouldNotBeNull();
            var errorMessage = badResult.Value as string;
            errorMessage.ShouldContain("Archived tours cannot be purchased");
        }

        [Fact]
        public void Purchase_includes_all_key_points_without_secrets()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 207);
            CleanupPurchaseTokens(scope, 207);
            var controller = CreateController(scope, "207");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.AddToCart(new AddToCartDto { TourId = -3 });

            // Get original tour key points count
            var originalTour = dbContext.Tours
                .Include(t => t.KeyPoints)
                .FirstOrDefault(t => t.Id == -3);
            var originalKeyPointsCount = originalTour?.KeyPoints.Count ?? 0;

            // Act
            var result = controller.PurchaseCart();
            var okResult = result.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert
            tokens.ShouldNotBeNull();
            var purchasedTour = tokens[0].Tour;
            purchasedTour.ShouldNotBeNull();
            purchasedTour!.KeyPoints.Count.ShouldBe(originalKeyPointsCount);
            
            // Verify each key point has required properties but no secret
            foreach (var keyPoint in purchasedTour.KeyPoints)
            {
                keyPoint.Latitude.ShouldNotBe(0);
                keyPoint.Longitude.ShouldNotBe(0);
                keyPoint.Name.ShouldNotBeNullOrEmpty();
                keyPoint.Description.ShouldNotBeNullOrEmpty();
                // Verify it's KeyPointWithoutSecretDto (no Secret property)
                var properties = keyPoint.GetType().GetProperties();
                properties.Any(p => p.Name == "Secret").ShouldBeFalse();
            }
        }

        [Fact]
        public void Purchase_includes_tour_durations_and_starting_point()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 208);
            CleanupPurchaseTokens(scope, 208);
            var controller = CreateController(scope, "208");

            controller.AddToCart(new AddToCartDto { TourId = -10 });

            // Act
            var result = controller.PurchaseCart();
            var okResult = result.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert
            tokens.ShouldNotBeNull();
            var purchasedTour = tokens[0].Tour;
            purchasedTour.ShouldNotBeNull();
            purchasedTour!.TourDurations.ShouldNotBeNull();
            purchasedTour.StartingPoint.ShouldNotBeNull();
            purchasedTour.StartingPoint?.Latitude.ShouldNotBe(0);
            purchasedTour.StartingPoint?.Longitude.ShouldNotBe(0);
        }

        [Fact]
        public void Purchase_multiple_tours_creates_separate_tokens()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 209);
            CleanupPurchaseTokens(scope, 209);
            var controller = CreateController(scope, "209");

            controller.AddToCart(new AddToCartDto { TourId = -10 });
            controller.AddToCart(new AddToCartDto { TourId = -5 });
            controller.AddToCart(new AddToCartDto { TourId = -3 });

            // Act
            var result = controller.PurchaseCart();
            var okResult = result.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert
            tokens.ShouldNotBeNull();
            tokens.Count.ShouldBe(3);
            
            // Each token should be unique
            var uniqueTourIds = tokens.Select(t => t.TourId).Distinct().Count();
            uniqueTourIds.ShouldBe(3);
            
            // All should have different IDs
            var uniqueTokenIds = tokens.Select(t => t.Id).Distinct().Count();
            uniqueTokenIds.ShouldBe(3);
        }

        /// <summary>
        /// Helper method to cleanup existing cart before test
        /// </summary>
        private static void CleanupCart(IServiceScope scope, long touristId)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            dbContext.ChangeTracker.Clear();
            
            var existingCart = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == touristId);

            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();
            }
        }

        /// <summary>
        /// Helper method to cleanup existing purchase tokens before test
        /// </summary>
        private static void CleanupPurchaseTokens(IServiceScope scope, long userId)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            dbContext.ChangeTracker.Clear();
            
            var existingTokens = dbContext.TourPurchaseTokens
                .Where(t => t.UserId == userId)
                .ToList();

            if (existingTokens.Any())
            {
                dbContext.TourPurchaseTokens.RemoveRange(existingTokens);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();
            }
        }

        private static ShoppingCartController CreateController(IServiceScope scope, string userId)
        {
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }

        private static ControllerContext BuildContext(string userId)
        {
            var claims = new[]
            {
                new Claim("id", userId),
                new Claim("personId", userId)
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            return new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = user
                }
            };
        }
    }
}
