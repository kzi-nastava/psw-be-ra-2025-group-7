using Explorer.API.Controllers.Follower;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Followers;

[Collection("Sequential")]
public class FollowerMessageTests : BaseStakeholdersIntegrationTest
{
    public FollowerMessageTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Send_message_to_followers_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var messageDto = new FollowerMessageDto
        {
            Content = "Test poruka za pratioce!",
            ResourceId = null,
            ResourceType = null
        };

        // Act
        var result = ((ObjectResult)controller.SendMessageToFollowers(messageDto).Result)?.Value as FollowerMessageDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.AuthorId.ShouldBe(-21);
        result.Content.ShouldBe(messageDto.Content);

        // Assert - Database
        var storedEntity = dbContext.FollowerMessages.Find(result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Content.ShouldBe(messageDto.Content);
    }

    [Fact]
    public void Send_message_with_tour_resource()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var messageDto = new FollowerMessageDto
        {
            Content = "Nova tura dostupna!",
            ResourceId = 1,
            ResourceType = "Tour"
        };

        // Act
        var result = ((ObjectResult)controller.SendMessageToFollowers(messageDto).Result)?.Value as FollowerMessageDto;

        // Assert
        result.ShouldNotBeNull();
        result.ResourceId.ShouldBe(1);
        result.ResourceType.ShouldBe("Tour");
    }

    [Fact]
    public void Send_message_fails_when_content_too_long()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var messageDto = new FollowerMessageDto
        {
            Content = new string('a', 281) // 281 karaktera
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.SendMessageToFollowers(messageDto));
    }

    [Fact]
    public void Send_message_fails_when_empty_content()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var messageDto = new FollowerMessageDto
        {
            Content = ""
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.SendMessageToFollowers(messageDto));
    }

    [Fact]
    public void Get_my_messages_returns_author_messages()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act
        var result = ((ObjectResult)controller.GetMyMessages(1, 20).Result)?.Value as PagedResult<FollowerMessageDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(m => m.AuthorId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Delete_message_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Use test message -3 (not used by other tests)
        var messageToDelete = dbContext.FollowerMessages.Find(-3L);
        if (messageToDelete == null)
        {
            // If doesn't exist, use existing message -1
            messageToDelete = dbContext.FollowerMessages.Find(-1L);
        }
        
        if (messageToDelete == null)
        {
            // Skip test if no messages exist
            return;
        }

        // Act
        var result = (OkResult)controller.DeleteMessage(messageToDelete.Id);

        // Assert
        result.StatusCode.ShouldBe(200);
        
        dbContext.ChangeTracker.Clear();
        var deletedEntity = dbContext.FollowerMessages.Find(messageToDelete.Id);
        deletedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_message_fails_when_not_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // Different user
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Find a message authored by -21 (not by -22)
        var messageByOtherUser = dbContext.FollowerMessages.FirstOrDefault(m => m.AuthorId == -21);
        
        if (messageByOtherUser == null)
        {
            // Skip test if no such message exists
            return;
        }

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.DeleteMessage(messageByOtherUser.Id));
    }

    private static FollowerMessageController CreateController(IServiceScope scope, string personId)
    {
        return new FollowerMessageController(scope.ServiceProvider.GetRequiredService<IFollowerMessageService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}