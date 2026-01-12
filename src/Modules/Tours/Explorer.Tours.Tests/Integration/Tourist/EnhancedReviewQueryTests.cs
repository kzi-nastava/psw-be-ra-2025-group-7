using Explorer.API.Controllers.Tourist.Tours;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class EnhancedReviewQueryTests : BaseToursIntegrationTest
{
    public EnhancedReviewQueryTests(ToursTestFactory factory) : base(factory) { }

    /// <summary>
    /// Requires seed:
    /// - tours."EnhancedReviews": Id = -8001, TourId = -3, TouristId = -11
    /// - optional: children rows
    /// </summary>
    [Fact]
    public async Task Retrieves_reviews_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var action = await controller.GetReviews(-3);
        var result = ExtractOkValue<List<EnhancedReviewDto>>(action);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);

        var first = result.First();
        first.DimensionRatings.ShouldNotBeNull();
        first.SentimentTags.ShouldNotBeNull();
        first.Pros.ShouldNotBeNull();
        first.Cons.ShouldNotBeNull();
        first.ImageUrls.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies sorting logic matches repository:
    /// Order by HelpfulVotes.Count desc, then CreatedAt desc
    /// </summary>
    [Fact]
    public async Task Retrieves_reviews_in_expected_order_most_helpful_then_newest()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var action = await controller.GetReviews(-3);
        var reviews = ExtractOkValue<List<EnhancedReviewDto>>(action);

        // Assert
        reviews.ShouldNotBeNull();
        reviews.Count.ShouldBeGreaterThan(0);

        var expectedOrder = dbContext.EnhancedReviews
            .Where(r => r.TourId == -3)
            .Select(r => new
            {
                r.Id,
                Helpful = dbContext.EnhancedReviewHelpfulVotes.Count(v => v.EnhancedReviewId == r.Id),
                r.CreatedAt
            })
            .OrderByDescending(x => x.Helpful)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .ToList();

        var actualOrder = reviews.Select(r => r.Id).ToList();
        actualOrder.ShouldBe(expectedOrder);
    }

    /// <summary>
    /// Requires seed:
    /// - at least 1 review for tour -3
    /// </summary>
    [Fact]
    public async Task Retrieves_summary_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var action = await controller.GetSummary(-3);
        var summary = ExtractOkValue<ReviewSummaryDto>(action);

        // Assert
        summary.ShouldNotBeNull();
        summary.AverageDimensions.ShouldNotBeNull();
        summary.OverallAverage.ShouldBeGreaterThan(0);
        summary.TopTags.ShouldNotBeNull();
        summary.TopPros.ShouldNotBeNull();
        summary.TopCons.ShouldNotBeNull();
    }

    /// <summary>
    /// Tour exists but has no reviews (use -2 from your seed tours)
    /// </summary>
    [Fact]
    public async Task Retrieves_empty_summary_when_no_reviews()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var action = await controller.GetSummary(-2);
        var summary = ExtractOkValue<ReviewSummaryDto>(action);

        // Assert
        summary.ShouldNotBeNull();
        summary.OverallAverage.ShouldBe(0);
        summary.TopTags.Count.ShouldBe(0);
        summary.TopPros.Count.ShouldBe(0);
        summary.TopCons.Count.ShouldBe(0);
    }

    private static TouristToursController CreateController(IServiceScope scope, string userId)
    {
        return new TouristToursController(
            scope.ServiceProvider.GetRequiredService<ITouristToursService>(),
            scope.ServiceProvider.GetRequiredService<IEnhancedReviewService>(),
            scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

    private static T ExtractOkValue<T>(ActionResult<T> action)
    {
        var ok = action.Result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.Value.ShouldNotBeNull();
        ok.Value.ShouldBeOfType<T>();
        return (T)ok.Value!;
    }
}
