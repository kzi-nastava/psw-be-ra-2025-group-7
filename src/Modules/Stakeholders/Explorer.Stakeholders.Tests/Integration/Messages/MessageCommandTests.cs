using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Messages
{

    [Collection("Sequential")]
    public class MessageCommandTests : BaseStakeholdersIntegrationTest
    {
        public MessageCommandTests(StakeholdersTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var message = new MessageDto
            {
                Content = "Hello!",
                SentByUserId = -11,
                SentToUserId = -12
            };

            // Act
            var res = ((ObjectResult)controller.SendMessage(message).Result)?.Value as MessageDto;

            // Assert - Response
            res.ShouldNotBeNull();
            res.Id.ShouldBeGreaterThan(0);
            res.Content.ShouldBe(message.Content);
            res.SentByUserId.ShouldBe(message.SentByUserId);
            res.SentToUserId.ShouldBe(message.SentToUserId);
            res.SentAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
            res.EditedAt.ShouldBe(DateTime.MinValue);

            // Assert - Database
            var dbMessage = dbContext.Messages.Find(res.Id);
            dbMessage.ShouldNotBeNull();
            dbMessage.Content.ShouldBe(message.Content);
            dbMessage.SentByUserId.ShouldBe(message.SentByUserId);
            dbMessage.SentToUserId.ShouldBe(message.SentToUserId);
            dbMessage.SentAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
            dbMessage.EditedAt.ShouldBe(DateTime.MinValue);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var message = new MessageDto
            {
                Content = "",
                SentByUserId = -11,
                SentToUserId = -12
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.SendMessage(message));
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var message = new MessageDto
            {
                Id = -1011,
                Content = "Updated content",
            };
            
            // Act
            var updateRes = ((ObjectResult)controller.EditMessage(message).Result!).Value as MessageDto;

            // Assert - Response
            updateRes.ShouldNotBeNull();
            updateRes.Id.ShouldBe(message.Id);
            updateRes.Content.ShouldBe("Updated content");
            updateRes.EditedAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));

            // Assert - Database
            var dbMessage = dbContext.Messages.Find(message.Id);
            dbMessage.ShouldNotBeNull();
            dbMessage.Content.ShouldBe("Updated content");
            dbMessage.EditedAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var message = new MessageDto
            {
                Id = -9999,
                Content = "Hello!",
                SentByUserId = -11,
                SentToUserId = -12
            };
            // Act & Assert
            Should.Throw<NotFoundException>(() => controller.EditMessage(message));
        }

        [Fact]
        public void Update_fails_invalid_data()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var message = new MessageDto
            {
                Id = -1021,
                Content = "",
                SentByUserId = -11,
                SentToUserId = -12
            };
            // Act & Assert

            Should.Throw<ArgumentException>(() => controller.EditMessage(message));

        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var message = new MessageDto
            {
                Content = "Hello!",
                SentByUserId = -11,
                SentToUserId = -12
            };
            var res = ((ObjectResult)controller.SendMessage(message).Result!).Value as MessageDto;

            // Act
            controller.DeleteMessage(res.Id);

            // Assert - Database
            var dbMessage = dbContext.Messages.Find(res.Id);
            dbMessage.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act & Assert
            Should.Throw<NotFoundException>(() => controller.DeleteMessage(-9999));
        }

        [Fact]
        public void Delete_fails_not_sender()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // print all message ids
            var messages = dbContext.Messages.ToList();

            // Act & Assert
            Should.Throw<UnauthorizedAccessException>(() => controller.DeleteMessage(-1023));
        }

        private static MessageController CreateController(IServiceScope scope)
        {
            return new MessageController(scope.ServiceProvider.GetRequiredService<IMessageService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
