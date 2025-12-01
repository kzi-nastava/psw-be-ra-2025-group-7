using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPurchaseTokenTests : BaseToursIntegrationTest
{
    public TourPurchaseTokenTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_purchase_token_for_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();
        long userId = -11; // New user
        long tourId = -3; // Published tour from test data

        // Act
        var result = service.Create(userId, tourId);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(userId);
        result.TourId.ShouldBe(tourId);
        result.PurchaseDate.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
    }

    [Fact]
    public void Fails_to_create_purchase_token_for_draft_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();
        long userId = -12;
        long tourId = -1; // Draft tour (Status = 0)

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => service.Create(userId, tourId));
        exception.Message.ShouldContain("cannot be purchased");
        exception.Message.ShouldContain("Published");
    }

    [Fact]
    public void Fails_to_create_duplicate_purchase_token()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();
        long userId = -1; // User who already purchased tour -3
        long tourId = -3;

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => service.Create(userId, tourId));
        exception.Message.ShouldContain("already purchased");
    }

    [Fact]
    public void Retrieves_user_purchases()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();
        long userId = -1;

        // Act
        var result = service.GetPagedByUser(0, 10, userId);

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThan(0);
        result.Results.ShouldAllBe(p => p.UserId == userId);
    }

    [Fact]
    public void Checks_if_user_purchased_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();

        // Act
        var hasPurchased = service.HasUserPurchasedTour(-1, -3);
        var hasNotPurchased = service.HasUserPurchasedTour(-99, -3);

        // Assert
        hasPurchased.ShouldBeTrue();
        hasNotPurchased.ShouldBeFalse();
    }

    [Fact]
    public void Gets_specific_purchase_token()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenService>();

        // Act
        var result = service.GetByUserAndTour(-1, -3);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(-1);
        result.TourId.ShouldBe(-3);
    }
}
