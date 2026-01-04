using Explorer.Payments.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class CouponTests
    {
        [Fact]
        public void Creates_coupon_with_valid_data()
        {
            // Arrange
            long authorId = -11;
            int discountPercentage = 20;
            DateTime? expirationDate = DateTime.UtcNow.AddMonths(6);

            // Act
            var coupon = new Coupon(authorId, discountPercentage, expirationDate, null);

            // Assert
            coupon.AuthorId.ShouldBe(authorId);
            coupon.DiscountPercentage.ShouldBe(discountPercentage);
            coupon.ExpirationDate.ShouldBe(expirationDate);
            coupon.TourId.ShouldBeNull();
            coupon.IsActive.ShouldBeTrue();
            coupon.Code.ShouldNotBeNullOrEmpty();
            coupon.Code.Length.ShouldBe(8);
        }

        [Fact]
        public void Creates_coupon_for_specific_tour()
        {
            // Arrange
            long authorId = -11;
            int discountPercentage = 15;
            long tourId = -1;

            // Act
            var coupon = new Coupon(authorId, discountPercentage, null, tourId);

            // Assert
            coupon.AuthorId.ShouldBe(authorId);
            coupon.TourId.ShouldBe(tourId);
            coupon.DiscountPercentage.ShouldBe(discountPercentage);
            coupon.ExpirationDate.ShouldBeNull();
        }

        [Fact]
        public void Creates_coupon_without_expiration_date()
        {
            // Arrange
            long authorId = -11;
            int discountPercentage = 30;

            // Act
            var coupon = new Coupon(authorId, discountPercentage, null, null);

            // Assert
            coupon.ExpirationDate.ShouldBeNull();
            coupon.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Generates_unique_8_character_code()
        {
            // Arrange & Act
            var coupon1 = new Coupon(-11, 20, null, null);
            var coupon2 = new Coupon(-11, 20, null, null);

            // Assert
            coupon1.Code.Length.ShouldBe(8);
            coupon2.Code.Length.ShouldBe(8);
            coupon1.Code.ShouldNotBe(coupon2.Code); // Codes should be unique
        }

        [Fact]
        public void IsValid_returns_true_for_active_non_expired_coupon()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, DateTime.UtcNow.AddDays(7), null);

            // Act
            var isValid = coupon.IsValid();

            // Assert
            isValid.ShouldBeTrue();
        }

        [Fact]
        public void IsValid_returns_false_for_expired_coupon()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, DateTime.UtcNow.AddDays(-1), null);

            // Act
            var isValid = coupon.IsValid();

            // Assert
            isValid.ShouldBeFalse();
        }

        [Fact]
        public void IsValid_returns_false_for_deactivated_coupon()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, DateTime.UtcNow.AddDays(7), null);
            coupon.Deactivate();

            // Act
            var isValid = coupon.IsValid();

            // Assert
            isValid.ShouldBeFalse();
        }

        [Fact]
        public void Deactivate_sets_IsActive_to_false()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, null, null);
            coupon.IsActive.ShouldBeTrue();

            // Act
            coupon.Deactivate();

            // Assert
            coupon.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void Activate_sets_IsActive_to_true()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, null, null);
            coupon.Deactivate();
            coupon.IsActive.ShouldBeFalse();

            // Act
            coupon.Activate();

            // Assert
            coupon.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void AppliesTo_returns_true_for_matching_tour()
        {
            // Arrange
            long tourId = -1;
            long authorId = -11;
            var coupon = new Coupon(authorId, 20, null, tourId);

            // Act
            var applies = coupon.AppliesTo(tourId, authorId);

            // Assert
            applies.ShouldBeTrue();
        }

        [Fact]
        public void AppliesTo_returns_false_for_non_matching_tour()
        {
            // Arrange
            long tourId = -1;
            long authorId = -11;
            var coupon = new Coupon(authorId, 20, null, tourId);

            // Act
            var applies = coupon.AppliesTo(-2, authorId);

            // Assert
            applies.ShouldBeFalse();
        }

        [Fact]
        public void AppliesTo_returns_true_for_any_tour_from_same_author()
        {
            // Arrange
            long authorId = -11;
            var coupon = new Coupon(authorId, 20, null, null); // No specific tour

            // Act
            var applies1 = coupon.AppliesTo(-1, authorId);
            var applies2 = coupon.AppliesTo(-3, authorId);

            // Assert
            applies1.ShouldBeTrue();
            applies2.ShouldBeTrue();
        }

        [Fact]
        public void AppliesTo_returns_false_for_different_author()
        {
            // Arrange
            long authorId = -11;
            var coupon = new Coupon(authorId, 20, null, null);

            // Act
            var applies = coupon.AppliesTo(-1, -12); // Different author

            // Assert
            applies.ShouldBeFalse();
        }

        [Fact]
        public void Update_changes_discount_percentage()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, null, null);

            // Act
            coupon.Update(30, DateTime.UtcNow.AddMonths(1), -1);

            // Assert
            coupon.DiscountPercentage.ShouldBe(30);
        }

        [Fact]
        public void Update_changes_expiration_date()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, null, null);
            var newExpirationDate = DateTime.UtcNow.AddMonths(3);

            // Act
            coupon.Update(20, newExpirationDate, null);

            // Assert
            coupon.ExpirationDate.ShouldBe(newExpirationDate);
        }

        [Fact]
        public void Update_changes_tour_id()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, null, null);

            // Act
            coupon.Update(20, null, -5);

            // Assert
            coupon.TourId.ShouldBe(-5);
        }

        [Fact]
        public void Throws_exception_for_invalid_discount_percentage_below_1()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => new Coupon(-11, 0, null, null))
                .Message.ShouldBe("Discount percentage must be between 1 and 100.");
        }

        [Fact]
        public void Throws_exception_for_invalid_discount_percentage_above_100()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => new Coupon(-11, 101, null, null))
                .Message.ShouldBe("Discount percentage must be between 1 and 100.");
        }

        [Fact]
        public void Throws_exception_for_invalid_author_id()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => new Coupon(0, 20, null, null))
                .Message.ShouldBe("Author ID must be valid.");
        }

        [Fact]
        public void Allows_negative_author_id_for_test_data()
        {
            // Arrange & Act
            var coupon = new Coupon(-11, 20, null, null);

            // Assert
            coupon.AuthorId.ShouldBe(-11);
        }

        [Fact]
        public void IsValid_returns_true_for_coupon_expiring_today()
        {
            // Arrange
            var coupon = new Coupon(-11, 20, DateTime.UtcNow.Date.AddHours(23).AddMinutes(59), null);

            // Act
            var isValid = coupon.IsValid();

            // Assert
            isValid.ShouldBeTrue();
        }

        [Fact]
        public void Multiple_coupons_generate_different_codes()
        {
            // Arrange & Act
            var codes = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                var coupon = new Coupon(-11, 20, null, null);
                codes.Add(coupon.Code);
            }

            // Assert
            codes.Count.ShouldBe(100); // All codes should be unique
        }

        [Fact]
        public void Coupon_code_contains_only_alphanumeric_characters()
        {
            // Arrange & Act
            var coupon = new Coupon(-11, 20, null, null);

            // Assert
            coupon.Code.All(c => char.IsLetterOrDigit(c)).ShouldBeTrue();
        }
    }
}
