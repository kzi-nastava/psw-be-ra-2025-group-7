using Explorer.API.Controllers.Follower;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Followers;

[Collection("Sequential")]
public class FollowerQueryTests : BaseStakeholdersIntegrationTest
{
    public FollowerQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_my_followers()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act
        var result = ((ObjectResult)controller.GetMyFollowers(1, 20).Result)?.Value as PagedResult<FollowerDto>;

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Results.ShouldNotBeEmpty();
        result.Results.All(f => f.FollowedId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_my_following()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act
        var result = ((ObjectResult)controller.GetMyFollowing(1, 20).Result)?.Value as PagedResult<FollowerDto>;

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Results.ShouldNotBeEmpty();
        result.Results.All(f => f.FollowerId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Checks_is_following_returns_true()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act
        var result = ((ObjectResult)controller.IsFollowing(-22).Result)?.Value as bool?;

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBeTrue();
    }

    [Fact]
    public void Checks_is_following_returns_false()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act
        var result = ((ObjectResult)controller.IsFollowing(-99).Result)?.Value as bool?;

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBeFalse();
    }

    private static FollowerController CreateController(IServiceScope scope, string personId)
    {
        return new FollowerController(scope.ServiceProvider.GetRequiredService<IFollowerService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}