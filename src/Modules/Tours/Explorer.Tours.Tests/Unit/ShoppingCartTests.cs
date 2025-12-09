using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class ShoppingCartTests
    {
        [Fact]
        public void Creates_empty_cart_for_tourist()
        {
            // Arrange
            long touristId = 1;

            // Act
            var cart = new ShoppingCart(touristId);

            // Assert
            cart.TouristId.ShouldBe(touristId);
            cart.Items.ShouldBeEmpty();
            cart.TotalPrice.ShouldBe(0);
        }

        [Fact]
        public void Adds_item_to_cart()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            long tourId = 10;
            string tourName = "Paris Tour";
            decimal price = 99.99m;

            // Act
            cart.AddItem(tourId, tourName, price);

            // Assert
            cart.Items.Count.ShouldBe(1);
            cart.Items[0].TourId.ShouldBe(tourId);
            cart.Items[0].TourName.ShouldBe(tourName);
            cart.Items[0].Price.ShouldBe(price);
            cart.TotalPrice.ShouldBe(price);
        }

        [Fact]
        public void Adds_multiple_items_and_calculates_total_price()
        {
            // Arrange
            var cart = new ShoppingCart(1);

            // Act
            cart.AddItem(1, "Tour A", 50.00m);
            cart.AddItem(2, "Tour B", 75.50m);
            cart.AddItem(3, "Tour C", 100.00m);

            // Assert
            cart.Items.Count.ShouldBe(3);
            cart.TotalPrice.ShouldBe(225.50m);
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            cart.AddItem(1, "Tour A", 50.00m);
            cart.AddItem(2, "Tour B", 75.50m);

            // Act - U unit testovima koristimo RemoveItemByTourId
            cart.RemoveItemByTourId(1);

            // Assert
            cart.Items.Count.ShouldBe(1);
            cart.Items[0].TourId.ShouldBe(2);
            cart.TotalPrice.ShouldBe(75.50m);
        }

        [Fact]
        public void Recalculates_total_price_after_removing_item()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            cart.AddItem(1, "Tour A", 50.00m);
            cart.AddItem(2, "Tour B", 75.50m);
            cart.AddItem(3, "Tour C", 100.00m);

            // Act - Ukloni Tour B (TourId = 2, cena 75.50m)
            cart.RemoveItemByTourId(2);

            // Assert
            cart.Items.Count.ShouldBe(2);
            cart.TotalPrice.ShouldBe(150.00m); // 50.00 + 100.00
            cart.Items.Any(i => i.TourId == 1).ShouldBeTrue(); // Tour A ostaje
            cart.Items.Any(i => i.TourId == 3).ShouldBeTrue(); // Tour C ostaje
            cart.Items.Any(i => i.TourId == 2).ShouldBeFalse(); // Tour B je uklonjena
        }

        [Fact]
        public void Throws_exception_when_adding_duplicate_tour()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            cart.AddItem(1, "Tour A", 50.00m);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => cart.AddItem(1, "Tour A", 50.00m))
                .Message.ShouldBe("Tour with ID 1 is already in the cart.");
        }

        [Fact]
        public void Throws_exception_when_removing_nonexistent_item()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            cart.AddItem(1, "Tour A", 50.00m);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => cart.RemoveItem(999))
                .Message.ShouldBe("Order item with ID 999 not found in cart.");
        }

        [Fact]
        public void Throws_exception_when_removing_nonexistent_tour_by_tour_id()
        {
            // Arrange
            var cart = new ShoppingCart(1);
            cart.AddItem(1, "Tour A", 50.00m);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => cart.RemoveItemByTourId(999))
                .Message.ShouldBe("Tour with ID 999 not found in cart.");
        }

        [Fact]
        public void Handles_zero_price_tours()
        {
            // Arrange
            var cart = new ShoppingCart(1);

            // Act
            cart.AddItem(1, "Free Tour", 0m);
            cart.AddItem(2, "Another Free Tour", 0m);

            // Assert
            cart.Items.Count.ShouldBe(2);
            cart.TotalPrice.ShouldBe(0m);
        }
    }
}
