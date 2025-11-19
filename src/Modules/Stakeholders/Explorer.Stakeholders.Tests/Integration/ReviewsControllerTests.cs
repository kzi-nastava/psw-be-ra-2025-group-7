using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Tests.Integration.Reviews;

[Collection("Sequential")] // Osigurava da testovi idu jedan za drugim
public class ReviewControllerTests : BaseStakeholdersIntegrationTest
{
    public ReviewControllerTests(StakeholdersTestFactory factory) : base(factory)
    {
        SeedTestData();
    }

    // Seed testnih podataka u bazu koju koriste testovi
    private void SeedTestData()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IReviewRepository>();

        // Dodaj testne review-e
        repo.Add(new Review(5, "Test review 1", -1));
        repo.Add(new Review(4, "Test review 2", -1));
    }

    // Helper metoda za kreiranje kontrolera sa servisom iz DI
    private static ReviewController CreateController(IServiceScope scope)
    {
        return new ReviewController(
            scope.ServiceProvider.GetRequiredService<IReviewService>()
        );
    }

    [Fact]
    public void Can_create_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var newReview = new CreateReviewDto
        {
            Rating = 5,
            Comment = "Super aplikacija!",
            PersonId = -1
        };

        var result = ((ObjectResult)controller.Create(newReview).Result).Value as ReviewDto;

        result.ShouldNotBeNull();
        result.Rating.ShouldBe(5);
        result.Comment.ShouldBe("Super aplikacija!");
        result.PersonId.ShouldBe(-1);
    }

    [Fact]
    public void Can_get_my_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetMyReview(-1).Result).Value as ReviewDto;

        result.ShouldNotBeNull();
        result.PersonId.ShouldBe(-1);
    }

    [Fact]
    public void Can_update_review()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IReviewRepository>();

        // Osiguraj da review postoji pre update-a
        var review = repo.Get(-1);
        if (review == null)
        {
            repo.Add(new Review(5, "Test review 1", -1));
        }

        var controller = CreateController(scope);

        var updateDto = new UpdateReviewDto
        {
            Rating = 4,
            Comment = "Dobar review!"
        };

        var result = ((ObjectResult)controller.Update(-1, updateDto).Result).Value as ReviewDto;

        result.ShouldNotBeNull();
        result.Rating.ShouldBe(4);
        result.Comment.ShouldBe("Dobar review!");
    }

    [Fact]
    public void Can_delete_review()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IReviewRepository>();

        // Seed review
        var review = repo.Get(-1);
        if (review == null)
        {
            repo.Add(new Review(5, "Test review 1", -1));
        }

        // Koristi isti scope za kontroler
        var controller = CreateController(scope);

        controller.Delete(-1);

        var deletedReview = repo.Get(-1);
        deletedReview.ShouldBeNull();
    }
}
