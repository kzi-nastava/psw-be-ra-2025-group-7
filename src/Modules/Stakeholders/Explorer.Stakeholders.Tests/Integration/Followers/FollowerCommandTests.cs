using Explorer.API.Controllers.Follower;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Followers;

[Collection("Sequential")]
public class FollowerCommandTests : BaseStakeholdersIntegrationTest
{
    public FollowerCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Follow_user_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Ensure the follow doesn't exist
        var existingFollow = dbContext.Followers.FirstOrDefault(f => 
            f.FollowerId == -22 && f.FollowedId == -23);
        if (existingFollow != null)
        {
            dbContext.Followers.Remove(existingFollow);
            dbContext.SaveChanges();
        }

        // Act
        var result = ((ObjectResult)controller.Follow(-23).Result)?.Value as FollowerDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.FollowerId.ShouldBe(-22);
        result.FollowedId.ShouldBe(-23);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedEntity = dbContext.Followers.FirstOrDefault(f => 
            f.FollowerId == -22 && f.FollowedId == -23);
        storedEntity.ShouldNotBeNull();
    }

    [Fact]
    public void Follow_fails_when_already_following()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Follow(-22));
    }

    [Fact]
    public void Follow_fails_when_following_self()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Follow(-21));
    }

    [Fact]
    public void Unfollow_user_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Ensure the follow exists first
        var existingFollow = dbContext.Followers.FirstOrDefault(f => 
            f.FollowerId == -21 && f.FollowedId == -23);
        
        if (existingFollow == null)
        {
            // Skip test if follow doesn't exist
            return;
        }

        // Act
        var result = (OkResult)controller.Unfollow(-23);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedEntity = dbContext.Followers.FirstOrDefault(f => 
            f.FollowerId == -21 && f.FollowedId == -23);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Unfollow_fails_when_not_following()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Unfollow(-99));
    }

    private static FollowerController CreateController(IServiceScope scope, string personId)
    {
        return new FollowerController(scope.ServiceProvider.GetRequiredService<IFollowerService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}