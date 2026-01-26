using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
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
    public class MessageQueryTests : BaseStakeholdersIntegrationTest
    {
        public MessageQueryTests(StakeholdersTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void GetsPagedContacts()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var actionResult = controller.GetPagedContacts().Result;
            var objectResult = actionResult as ObjectResult;
            var res = objectResult?.Value as PagedResult<ContactDto>;

            // Assert
            res.ShouldNotBeNull();
            res.TotalCount.ShouldBe(3);

            res.Results.Select(c => c.UserId).ShouldContain(-11);
            res.Results.Select(c => c.UserId).ShouldContain(-12);
            res.Results.Select(c => c.UserId).ShouldContain(-21);
        }

        [Fact]
        public void GetsPagedRecent()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            // Act
            var res = ((ObjectResult)controller.GetPagedRecent().Result)?.Value as PagedResult<MessageDto>;
            // Assert
            res.ShouldNotBeNull();
            res.TotalCount.ShouldBe(3);
            res.Results.Count.ShouldBe(3);
        }

        [Fact]
        public void GetsPagedByConversation()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            // Act
            var res = ((ObjectResult)controller.GetPagedByConversation(-12).Result)?.Value as PagedResult<MessageDto>;
            // Assert
            res.ShouldNotBeNull();
            res.TotalCount.ShouldBe(1);
            res.Results.Count.ShouldBe(1);
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
