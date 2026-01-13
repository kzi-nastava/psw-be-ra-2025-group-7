using Explorer.API.Controllers.Tourist.Tours;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class EnhancedReviewCommandTests : BaseToursIntegrationTest
{
    // Koristi postojecu turu iz seed-a (b-tours-insert.sql obicno ima -3)
    private const long TourId = -3;

    public EnhancedReviewCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Creates_review()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Unique test user (da ne koliduje sa seed-om)
        var touristId = -1001L;

        await EnsureCanCreateReview(dbContext, touristId, TourId);

        var controller = CreateController(scope, touristId.ToString());
        var dto = ValidDto();

        var action = await controller.CreateReview(TourId, dto);
        var reviewId = ExtractOkValue<long>(action);

        reviewId.ShouldBeGreaterThan(0);

        var stored = dbContext.EnhancedReviews.FirstOrDefault(r => r.Id == reviewId);
        stored.ShouldNotBeNull();
        stored.TourId.ShouldBe(TourId);
        stored.TouristId.ShouldBe(touristId);
        stored.OverallRating.ShouldBe(5);
    }

    [Fact]
    public void Create_fails_when_not_purchased()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2001"); // nema token
        var dto = ValidDto();

        Should.Throw<ForbiddenException>(() =>
            controller.CreateReview(TourId, dto).GetAwaiter().GetResult());
    }

    [Fact]
    public async Task Create_fails_when_already_reviewed()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var touristId = -1002L;

        // 1) omoguci kreiranje
        await EnsureCanCreateReview(dbContext, touristId, TourId);

        var controller = CreateController(scope, touristId.ToString());
        var dto = ValidDto();

        // 2) napravi prvi review
        var firstAction = await controller.CreateReview(TourId, dto);
        var firstId = ExtractOkValue<long>(firstAction);
        firstId.ShouldBeGreaterThan(0);

        // 3) drugi put mora da pukne: already reviewed
        Should.Throw<EntityValidationException>(() =>
            controller.CreateReview(TourId, dto).GetAwaiter().GetResult());
    }

    [Fact]
    public async Task Create_fails_when_invalid_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var touristId = -1003L;
        await EnsureCanCreateReview(dbContext, touristId, TourId);

        var controller = CreateController(scope, touristId.ToString());
        var dto = ValidDto();
        dto.OverallRating = 0;

        Should.Throw<EntityValidationException>(() =>
            controller.CreateReview(TourId, dto).GetAwaiter().GetResult());
    }

    [Fact]
    public async Task Create_fails_when_no_tags()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var touristId = -1004L;
        await EnsureCanCreateReview(dbContext, touristId, TourId);

        var controller = CreateController(scope, touristId.ToString());
        var dto = ValidDto();
        dto.SentimentTags = new List<string>();

        Should.Throw<EntityValidationException>(() =>
            controller.CreateReview(TourId, dto).GetAwaiter().GetResult());
    }

    [Fact]
    public async Task Toggle_helpful_fails_on_own_review()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var authorId = -1005L;
        await EnsureCanCreateReview(dbContext, authorId, TourId);

        var controller = CreateController(scope, authorId.ToString());

        // napravi review kao authorId
        var created = await controller.CreateReview(TourId, ValidDto());
        var reviewId = ExtractOkValue<long>(created);

        // author pokusava helpful na svoj review -> mora da pukne
        Should.Throw<EntityValidationException>(() =>
            controller.ToggleHelpful(reviewId).GetAwaiter().GetResult());
    }

    [Fact]
    public async Task Toggle_helpful_toggles_for_other_user()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var authorId = -1006L;
        var voterId = -1007L;

        await EnsureCanCreateReview(dbContext, authorId, TourId);

        var authorController = CreateController(scope, authorId.ToString());
        var created = await authorController.CreateReview(TourId, ValidDto());
        var reviewId = ExtractOkValue<long>(created);

        var voterController = CreateController(scope, voterId.ToString());

        var before = dbContext.EnhancedReviewHelpfulVotes.Count(v => v.EnhancedReviewId == reviewId);

        var action1 = await voterController.ToggleHelpful(reviewId);
        var count1 = ExtractOkValue<int>(action1);

        var now1 = dbContext.EnhancedReviewHelpfulVotes.Count(v => v.EnhancedReviewId == reviewId);
        (now1 == before + 1 || now1 == before - 1).ShouldBeTrue();
        count1.ShouldBe(now1);

        var action2 = await voterController.ToggleHelpful(reviewId);
        var count2 = ExtractOkValue<int>(action2);

        var now2 = dbContext.EnhancedReviewHelpfulVotes.Count(v => v.EnhancedReviewId == reviewId);
        now2.ShouldBe(before);
        count2.ShouldBe(now2);
    }

    // ----------------- Helpers -----------------

    private static EnhancedReviewDto ValidDto()
    {
        return new EnhancedReviewDto
        {
            OverallRating = 5,
            DimensionRatings = new DimensionRatingsDto
            {
                GuideQuality = 5,
                ValueForMoney = 4,
                RouteScenery = 5,
                Difficulty = 3,
                GroupSize = 4
            },
            SentimentTags = new List<string> { "FriendlyGuide" },
            Pros = new List<string> { "Nice guide" },
            Cons = new List<string> { "Too crowded" },
            TextReview = "Great tour!"
        };
    }

    private static TouristToursController CreateController(IServiceScope scope, string userId)
    {
        return new TouristToursController(
            scope.ServiceProvider.GetRequiredService<ITouristToursService>(),
            scope.ServiceProvider.GetRequiredService<IEnhancedReviewService>(),
            scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>())
        {
            ControllerContext = BuildTouristContext(userId)
        };
    }

    private static ControllerContext BuildTouristContext(string userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim("id", userId) },
                "TestAuth"));

        return new ControllerContext { HttpContext = httpContext };
    }

    private static T ExtractOkValue<T>(ActionResult<T> action)
    {
        var ok = action.Result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.Value.ShouldNotBeNull();
        ok.Value.ShouldBeOfType<T>();
        return (T)ok.Value!;
    }

    /// <summary>
    /// Obavezno: postoji purchase token za (userId, tourId) i nema vec postojeceg EnhancedReview za taj par.
    /// Ne dira seed fajlove, radi direktno SQL nad test bazom.
    /// </summary>
    private static async Task EnsureCanCreateReview(ToursContext db, long userId, long tourId)
    {
        // 1) obriši eventualni postojeći review za taj user/tour (ako ga je seed negde ubacio ili test ranije napravio)
        // prvo child tabele, pa parent
        await db.Database.ExecuteSqlRawAsync(@"
DELETE FROM tours.""EnhancedReviewHelpfulVotes"" WHERE ""EnhancedReviewId"" IN
    (SELECT ""Id"" FROM tours.""EnhancedReviews"" WHERE ""TourId"" = {0} AND ""TouristId"" = {1});
DELETE FROM tours.""EnhancedReviewImages"" WHERE ""EnhancedReviewId"" IN
    (SELECT ""Id"" FROM tours.""EnhancedReviews"" WHERE ""TourId"" = {0} AND ""TouristId"" = {1});
DELETE FROM tours.""EnhancedReviewTags"" WHERE ""EnhancedReviewId"" IN
    (SELECT ""Id"" FROM tours.""EnhancedReviews"" WHERE ""TourId"" = {0} AND ""TouristId"" = {1});
DELETE FROM tours.""EnhancedReviewPros"" WHERE ""EnhancedReviewId"" IN
    (SELECT ""Id"" FROM tours.""EnhancedReviews"" WHERE ""TourId"" = {0} AND ""TouristId"" = {1});
DELETE FROM tours.""EnhancedReviewCons"" WHERE ""EnhancedReviewId"" IN
    (SELECT ""Id"" FROM tours.""EnhancedReviews"" WHERE ""TourId"" = {0} AND ""TouristId"" = {1});
DELETE FROM tours.""EnhancedReviews"" WHERE ""TourId"" = {0} AND ""TouristId"" = {1};
", tourId, userId);

        // 2) obezbedi purchase token (ako vec postoji, ne dupliciraj)
        await db.Database.ExecuteSqlRawAsync(@"
INSERT INTO tours.""TourPurchaseTokens"" (""UserId"", ""TourId"", ""PurchaseDate"")
SELECT {0}, {1}, NOW()
WHERE NOT EXISTS (
    SELECT 1 FROM tours.""TourPurchaseTokens""
    WHERE ""UserId"" = {0} AND ""TourId"" = {1}
);
", userId, tourId);
    }
}
