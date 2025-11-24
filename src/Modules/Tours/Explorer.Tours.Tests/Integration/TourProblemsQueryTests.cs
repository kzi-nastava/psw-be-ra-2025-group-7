using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourProblemQueryTests : BaseToursIntegrationTest
    {
        public TourProblemQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_mine()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetMine(0, 10).Result)?.Value as PagedResult<TourProblemDto>;

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
            result.Results.Count.ShouldBeLessThanOrEqualTo(10);
        }

        private static TourProblemController CreateController(IServiceScope scope)
        {
            return new TourProblemController(scope.ServiceProvider.GetRequiredService<ITourProblemService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
