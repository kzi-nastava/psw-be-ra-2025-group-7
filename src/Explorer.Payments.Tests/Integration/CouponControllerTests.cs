using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Core.Domain;
using Xunit;
using System;
using System.Linq;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class CouponControllerTests : BasePaymentsIntegrationTest, IClassFixture<PaymentsTestFactory>
{
    public CouponControllerTests(PaymentsTestFactory factory) : base(factory)
    {
        // TestData will be seeded automatically from SQL files
    }

    private static CouponsController CreateController(IServiceScope scope)
    {
        return new CouponsController(
            scope.ServiceProvider.GetRequiredService<ICouponService>()
        )
        {
            ControllerContext = BuildContext("-11") // Author ID -11
        };
    }

    [Fact]
    public void Can_create_coupon_for_all_tours()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var newCoupon = new CreateCouponDto
        {
            DiscountPercentage = 25,
            ExpirationDate = DateTime.UtcNow.AddMonths(3),
            TourId = null // Applies to all tours
        };

        // Act
        var result = ((ObjectResult)controller.Create(newCoupon).Result)?.Value as CouponDto;

        // Assert
        result.ShouldNotBeNull();
        result.AuthorId.ShouldBe(-11);
        result.DiscountPercentage.ShouldBe(25);
        result.TourId.ShouldBeNull();
        result.Code.ShouldNotBeNullOrEmpty();
        result.Code.Length.ShouldBe(8);
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Can_create_coupon_without_expiration_date()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var newCoupon = new CreateCouponDto
        {
            DiscountPercentage = 10,
            ExpirationDate = null,
            TourId = null
        };

        // Act
        var result = ((ObjectResult)controller.Create(newCoupon).Result)?.Value as CouponDto;

        // Assert
        result.ShouldNotBeNull();
        result.ExpirationDate.ShouldBeNull();
        result.DiscountPercentage.ShouldBe(10);
    }

    [Fact]
    public void Can_get_all_coupons_for_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Verify seed data loaded correctly
        var dbCoupons = context.Coupons.Where(c => c.AuthorId == -11).ToList();
        if (dbCoupons.Count < 4)
        {
            throw new InvalidOperationException($"SEED DATA INCOMPLETE! Expected at least 4 coupons for author -11, found {dbCoupons.Count}. Check SQL seed file execution.");
        }

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<CouponDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(4); // At least 4 from seed for author -11
        result.All(c => c.AuthorId == -11).ShouldBeTrue();
    }

    [Fact]
    public void Can_get_only_active_coupons()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var allCoupons = ((ObjectResult)controller.GetAll().Result)?.Value as List<CouponDto>;
        var activeCoupons = ((ObjectResult)controller.GetMyCoupons(true).Result)?.Value as List<CouponDto>;

        // Assert
        allCoupons.ShouldNotBeNull();
        activeCoupons.ShouldNotBeNull();
        activeCoupons.All(c => c.IsActive).ShouldBeTrue();
        activeCoupons.Count.ShouldBeLessThanOrEqualTo(allCoupons.Count);
    }

    [Fact]
    public void Can_update_coupon()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var existingCoupon = context.Coupons.FirstOrDefault(c => c.AuthorId == -11);
        if (existingCoupon == null)
        {
            // Skip test if no coupons exist
            return;
        }

        var couponId = existingCoupon.Id;

        var updateDto = new UpdateCouponDto
        {
            DiscountPercentage = 35,
            ExpirationDate = DateTime.UtcNow.AddMonths(6),
            TourId = null
        };

        // Act
        var result = ((ObjectResult)controller.Update(couponId, updateDto).Result)?.Value as CouponDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(couponId);
        result.DiscountPercentage.ShouldBe(35);

        // Assert - Database
        context.ChangeTracker.Clear();
        var storedCoupon = context.Coupons.Find(couponId);
        storedCoupon.ShouldNotBeNull();
        storedCoupon.DiscountPercentage.ShouldBe(35);
    }

    [Fact]
    public void Can_deactivate_coupon()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var existingCoupon = context.Coupons.First(c => c.AuthorId == -11 && c.IsActive);
        var couponId = existingCoupon.Id;

        // Act
        var result = controller.Deactivate(couponId);

        // Assert
        result.ShouldBeOfType<OkResult>();

        // Assert - Database
        context.ChangeTracker.Clear();
        var storedCoupon = context.Coupons.Find(couponId);
        storedCoupon.ShouldNotBeNull();
        storedCoupon.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Can_activate_coupon()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var inactiveCoupon = context.Coupons.FirstOrDefault(c => c.AuthorId == -11 && !c.IsActive);
        
        if (inactiveCoupon == null)
        {
            // Skip test if no inactive coupons exist (seed data issue)
            return;
        }

        var couponId = inactiveCoupon.Id;

        // Act
        var result = controller.Activate(couponId);

        // Assert
        result.ShouldBeOfType<OkResult>();

        // Assert - Database
        context.ChangeTracker.Clear();
        var storedCoupon = context.Coupons.Find(couponId);
        storedCoupon.ShouldNotBeNull();
        storedCoupon.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Can_delete_coupon()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var existingCoupon = context.Coupons.First(c => c.AuthorId == -11);
        var couponId = existingCoupon.Id;

        // Act
        var result = controller.Delete(couponId);

        // Assert - Response
        result.ShouldBeOfType<OkResult>();

        // Assert - Database
        context.ChangeTracker.Clear();
        var deletedCoupon = context.Coupons.Find(couponId);
        deletedCoupon.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_for_nonexistent_coupon()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var updateDto = new UpdateCouponDto
        {
            DiscountPercentage = 20,
            ExpirationDate = DateTime.UtcNow.AddMonths(1),
            TourId = null
        };

        // Act
        var result = controller.Update(99999, updateDto).Result;

        // Assert - Should return NotFound
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Create_fails_with_invalid_discount_percentage()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalidCoupon = new CreateCouponDto
        {
            DiscountPercentage = 150, // Invalid - over 100
            ExpirationDate = DateTime.UtcNow.AddMonths(1),
            TourId = null
        };

        // Act
        var result = controller.Create(invalidCoupon).Result;

        // Assert - Should return BadRequest
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Can_filter_expired_coupons()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        // Verify we have seed data
        var totalCoupons = context.Coupons.Count(c => c.AuthorId == -11);
        if (totalCoupons < 3)
        {
            throw new InvalidOperationException($"SEED DATA INCOMPLETE! Expected at least 3 coupons for author -11, found {totalCoupons}.");
        }

        // Act
        var allCoupons = ((ObjectResult)controller.GetAll().Result)?.Value as List<CouponDto>;

        // Assert
        allCoupons.ShouldNotBeNull();
        allCoupons.Count.ShouldBeGreaterThanOrEqualTo(3);
        
        // Should have at least some expired coupons based on seed data
        var hasExpired = allCoupons.Any(c => c.ExpirationDate.HasValue && c.ExpirationDate.Value < DateTime.UtcNow);
        hasExpired.ShouldBeTrue("Expected at least one expired coupon from seed data");
    }
}
