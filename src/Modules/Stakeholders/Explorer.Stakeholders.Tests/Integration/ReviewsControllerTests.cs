using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests.Integration.Reviews;

[Collection("Sequential")] // Osigurava da testovi idu jedan za drugim
public class ReviewControllerTests : BaseStakeholdersIntegrationTest
{
    public ReviewControllerTests(StakeholdersTestFactory factory) : base(factory)
    {
        SeedTestData();
    }

    
    private void SeedTestData()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        
        context.Reviews.RemoveRange(context.Reviews);
        context.SaveChanges();

        
        context.Reviews.Add(new Review(5, "Test review 1", -11));
        context.Reviews.Add(new Review(4, "Test review 2", -12));
        context.SaveChanges();
    }

    
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
            PersonId = -11  // koristi validan PersonId iz seed baze
        };

        var result = ((ObjectResult)controller.Create(newReview).Result).Value as ReviewDto;

        result.ShouldNotBeNull();
        result.Rating.ShouldBe(5);
        result.Comment.ShouldBe("Super aplikacija!");
        result.PersonId.ShouldBe(-11); // promenjeno sa -1 na -11
    }

    [Fact]
    public void Can_get_my_review()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetMyReview(-11).Result).Value as ReviewDto;

        result.ShouldNotBeNull();
        result.PersonId.ShouldBe(-11);
    }

    [Fact]
    public void Can_update_review()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IReviewRepository>();

        
        var review = repo.GetAll().FirstOrDefault(r => r.PersonId == -11);
        if (review == null)
        {
            repo.Add(new Review(5, "Test review 1", -11));
        }

        var controller = CreateController(scope);

        var updateDto = new UpdateReviewDto
        {
            Rating = 4,
            Comment = "Dobar review!"
        };

        var result = ((ObjectResult)controller.Update((int)review.Id, updateDto).Result).Value as ReviewDto;

        result.ShouldNotBeNull();
        result.Rating.ShouldBe(4);
        result.Comment.ShouldBe("Dobar review!");
    }

    [Fact]
    public void Can_delete_review()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IReviewRepository>();

        
        var review = repo.GetAll().FirstOrDefault(r => r.PersonId == -11);
        if (review == null)
        {
            repo.Add(new Review(5, "Test review 1", -11));
            review = repo.GetAll().First(r => r.PersonId == -11);
        }

        var controller = CreateController(scope);

        controller.Delete((int)review.Id);

        var deletedReview = repo.Get((int)review.Id);
        deletedReview.ShouldBeNull();
    }
}
