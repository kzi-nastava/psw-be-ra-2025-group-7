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
    public class ShoppingCartIntegrationTests : BaseToursIntegrationTest
    {
        public ShoppingCartIntegrationTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_empty_cart_for_tourist()
        {
            // Arrange - koristi turistu bez korpe
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 100);
            var controller = CreateController(scope, "100");

            // Act
            var result = controller.GetMyCart();
            var okResult = result.Result as OkObjectResult;
            var cart = okResult?.Value as ShoppingCartDto;

            // Assert
            cart.ShouldNotBeNull();
            cart.TouristId.ShouldBe(100);
            cart.Items.ShouldBeEmpty();
            cart.TotalPrice.ShouldBe(0);
        }

        [Fact]
        public void Adds_published_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 101);
            var controller = CreateController(scope, "101");

            var addDto = new AddToCartDto { TourId = -3 };

            // Act
            var result = controller.AddToCart(addDto);
            var okResult = result.Result as OkObjectResult;
            var cart = okResult?.Value as ShoppingCartDto;

            // Assert
            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(1);
            cart.Items[0].TourId.ShouldBe(-3);
            cart.TotalPrice.ShouldBeGreaterThanOrEqualTo(0);

            // Assert Database
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            dbContext.ChangeTracker.Clear(); // ? Clear tracked entities
            var storedCart = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == 101);
            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
        }

        [Fact]
        public void Calculates_total_price_correctly()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 102);
            var controller = CreateController(scope, "102");

            // Act - dodaj više tura
            controller.AddToCart(new AddToCartDto { TourId = -3 });
            var result = controller.AddToCart(new AddToCartDto { TourId = -5 });
            var okResult = result.Result as OkObjectResult;
            var cart = okResult?.Value as ShoppingCartDto;

            // Assert
            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(2);
            var expectedTotal = cart.Items.Sum(i => i.Price);
            cart.TotalPrice.ShouldBe(expectedTotal);
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 103);
            var controller = CreateController(scope, "103");

            controller.AddToCart(new AddToCartDto { TourId = -3 });
            var addResult = controller.AddToCart(new AddToCartDto { TourId = -5 });
            var addOkResult = addResult.Result as OkObjectResult;
            var cartAfterAdd = addOkResult?.Value as ShoppingCartDto;
            var itemToRemove = cartAfterAdd.Items[0].Id;

            // Act
            var result = controller.RemoveFromCart(itemToRemove);
            var okResult = result.Result as OkObjectResult;
            var cart = okResult?.Value as ShoppingCartDto;

            // Assert
            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(1);
        }

        [Fact]
        public void Recalculates_total_price_after_removing_item()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 104);
            var controller = CreateController(scope, "104");

            controller.AddToCart(new AddToCartDto { TourId = -3 });
            var addResult = controller.AddToCart(new AddToCartDto { TourId = -5 });
            var addOkResult = addResult.Result as OkObjectResult;
            var cartAfterAdd = addOkResult?.Value as ShoppingCartDto;
            var itemToRemove = cartAfterAdd.Items[0].Id;

            // Act
            var result = controller.RemoveFromCart(itemToRemove);
            var okResult = result.Result as OkObjectResult;
            var cart = okResult?.Value as ShoppingCartDto;

            // Assert
            cart.ShouldNotBeNull();
            cart.TotalPrice.ShouldBe(cart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void Cannot_add_archived_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 105);
            var controller = CreateController(scope, "105");

            // Act
            var result = controller.AddToCart(new AddToCartDto { TourId = -100 });
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            badResult.ShouldNotBeNull();
            var errorMessage = badResult.Value as string;
            errorMessage.ShouldContain("Archived tours cannot be purchased");
        }

        [Fact]
        public void Cannot_add_draft_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 106);
            var controller = CreateController(scope, "106");

            // Act
            var result = controller.AddToCart(new AddToCartDto { TourId = -101 });
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            badResult.ShouldNotBeNull();
            var errorMessage = badResult.Value as string;
            errorMessage.ShouldContain("Only published tours can be added to cart");
        }

        [Fact]
        public void Clears_entire_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            CleanupCart(scope, 107);
            var controller = CreateController(scope, "107");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.AddToCart(new AddToCartDto { TourId = -3 });
            controller.AddToCart(new AddToCartDto { TourId = -5 });

            // Act
            var result = controller.ClearCart();

            // Assert
            result.ShouldBeOfType<OkResult>();

            // Assert Database
            dbContext.ChangeTracker.Clear(); // ? Clear tracked entities
            var storedCart = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == 107);
            storedCart.ShouldBeNull();
        }

        /// <summary>
        /// Helper metoda za ?iš?enje postoje?e korpe pre testa
        /// </summary>
        private static void CleanupCart(IServiceScope scope, long touristId)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            
            // ? Clear EF Core cache pre provere
            dbContext.ChangeTracker.Clear();
            
            var existingCart = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == touristId);

            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
                
                // ? Clear cache nakon brisanja
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
