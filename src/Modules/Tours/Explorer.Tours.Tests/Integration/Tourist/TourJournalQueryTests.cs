using Explorer.API.Controllers.Tourist.Tours;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
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

        // Assert - Response
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeEmpty();
        result.Results.Count.ShouldBe(2); // Turista -21 ima 2 dnevnika
        result.TotalCount.ShouldBe(2);
        result.Results.All(tj => tj.TouristId == -21).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_journals_with_pagination()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyTourJournals(1, 1).Result)?.Value as PagedResult<TourJournalDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(1); // Samo 1 po stranici
        result.TotalCount.ShouldBe(2); // Ukupno 2
        result.Results.First().TouristId.ShouldBe(-21);
    }

    [Fact]
    public void Retrieves_no_journals_for_tourist_without_journals()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerForDifferentUser(scope);

        // Act
        var result = ((ObjectResult)controller.GetMyTourJournals(0, 0).Result)?.Value as PagedResult<TourJournalDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public void Retrieves_journals_with_correct_properties()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = ((ObjectResult)controller.GetMyTourJournals(0, 0).Result)?.Value as PagedResult<TourJournalDto>;

        // Assert
        result.ShouldNotBeNull();
        var firstJournal = result.Results.FirstOrDefault(tj => tj.Id == -1);
        firstJournal.ShouldNotBeNull();
        firstJournal.Name.ShouldBe("Moja avantura u Beogradu");
        firstJournal.Country.ShouldBe("Srbija");
        firstJournal.City.ShouldBe("Beograd");
        firstJournal.Status.ShouldBe("Draft");

        var secondJournal = result.Results.FirstOrDefault(tj => tj.Id == -2);
        secondJournal.ShouldNotBeNull();
        secondJournal.Name.ShouldBe("Putovanje kroz Evropu");
        secondJournal.Status.ShouldBe("Published");
        secondJournal.City.ShouldBeNull();
    }

    private static TourJournalController CreateController(IServiceScope scope)
    {
        return new TourJournalController(scope.ServiceProvider.GetRequiredService<ITourJournalService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }

    private static TourJournalController CreateControllerForDifferentUser(IServiceScope scope)
    {
        return new TourJournalController(scope.ServiceProvider.GetRequiredService<ITourJournalService>())
        {
            ControllerContext = BuildContext("-99")
        };
    }
}
