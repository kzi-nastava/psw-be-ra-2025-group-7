using Explorer.API.Controllers.Tourist.Tours;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourJournalQueryTests : BaseToursIntegrationTest
{
    public TourJournalQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_by_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyTourJournals(0, 0).Result)?.Value as PagedResult<TourJournalDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.All(tj => tj.TouristId == -21).ShouldBeTrue();
    }

    private static TourJournalController CreateController(IServiceScope scope)
    {
        return new TourJournalController(scope.ServiceProvider.GetRequiredService<ITourJournalService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }
}
