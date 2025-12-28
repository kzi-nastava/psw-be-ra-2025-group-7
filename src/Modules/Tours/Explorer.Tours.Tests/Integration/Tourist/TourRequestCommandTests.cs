using Explorer.API.Controllers.Tourist.Tours;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourRequestCommandTests : BaseToursIntegrationTest
{
    public TourRequestCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new CreateTourRequestDto
        {
            Title = "Nova planinarska tura",
            Description = "Tražim vodiča za trodnevnu planinarsku turu u Nacionalnom parku Tara. Interesuje me tura srednje težine.",
            Budget = 10000,
            Latitude = 43.9159,
            Longitude = 19.3520,
            Radius = 30,
            PreferredDifficulty = 1,
            NumberOfParticipants = 3,
            PreferredDate = DateTime.UtcNow.AddDays(60)
        };

        // Act
        var actionResult = controller.Create(dto);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.TouristId.ShouldBe(-21);
        result.Title.ShouldBe(dto.Title);
        result.Status.ShouldBe(0); // Open
        result.ResponseCount.ShouldBe(0);
        result.DaysUntilExpiration.ShouldBeInRange(29, 30); // Allow 29 or 30 days

        // Assert - Database
        var stored = dbContext.TourRequests.Find(result.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Budget.ShouldBe(dto.Budget);
    }

    [Fact]
    public void Create_fails_with_short_title()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestDto
        {
            Title = "Kratko", // Manje od 10 karaktera
            Description = "Opis koji je dovoljno dugačak da prođe validaciju i ispuni zahtev od minimalno 50 karaktera.",
            Budget = 5000
        };

        // Act
        var actionResult = controller.Create(dto);

        // Debug - pogledaj šta se vraća
        var createdResult = actionResult.Result as CreatedAtActionResult;
        var badRequestResult = actionResult.Result as BadRequestObjectResult;
        var objectResult = actionResult.Result as ObjectResult;

        // Assert - trebalo bi da je BadRequest
        actionResult.Result.ShouldNotBeNull();
        badRequestResult.ShouldNotBeNull($"Expected BadRequestObjectResult but got {actionResult.Result?.GetType().Name}");
        badRequestResult.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Create_fails_with_short_description()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestDto
        {
            Title = "Validan naslov ture",
            Description = "Kratak opis", // Manje od 50 karaktera
            Budget = 5000
        };

        // Act
        var actionResult = controller.Create(dto);

        // Assert
        actionResult.Result.ShouldNotBeNull();
        var badRequestResult = actionResult.Result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull($"Expected BadRequestObjectResult but got {actionResult.Result?.GetType().Name}");
        badRequestResult.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Create_fails_on_sixth_request_same_day()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-99"); // Koristi drugog turista da ne smeta drugim testovima

        var dto = new CreateTourRequestDto
        {
            Title = "Test rate limit zahtev broj ",
            Description = "Testiramo rate limiting funkcionalnost koja omogućava maksimalno 5 zahteva dnevno po turistu.",
            Budget = 5000
        };

        // Act - Kreiraj 5 zahteva uspešno
        for (int i = 1; i <= 5; i++)
        {
            dto.Title = $"Test rate limit zahtev broj {i}";
            var tempResult = controller.Create(dto);
            var tempDto = ((ObjectResult)tempResult.Result)?.Value as TourRequestDto;
            tempDto.ShouldNotBeNull();
        }

        // Šesti zahtev treba da fejluje
        dto.Title = "Test rate limit zahtev broj 6";
        var actionResult = controller.Create(dto);
        var result = actionResult.Result as ObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Updates_tour_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new UpdateTourRequestDto
        {
            Id = -1, // Open request od tourist -21
            Title = "Ažuriran naslov ture",
            Description = "Ažuriran opis ture sa svim potrebnim detaljima i informacijama koje su potrebne za validaciju.",
            Budget = 16000,
            NumberOfParticipants = 5
        };

        // Act
        var actionResult = controller.Update(-1, dto);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Title.ShouldBe(dto.Title);
        result.Budget.ShouldBe(dto.Budget);
        result.NumberOfParticipants.ShouldBe(5);

        // Assert - Database
        var stored = dbContext.TourRequests.Find((long)-1);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Budget.ShouldBe(dto.Budget);
    }

    [Fact]
    public void Update_fails_for_non_open_status()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // Owner je -22

        var dto = new UpdateTourRequestDto
        {
            Id = -4, // Fulfilled request
            Title = "Pokušaj izmene fulfilled zahteva",
            Description = "Ovaj pokušaj izmene bi trebalo da ne uspe jer je zahtev već fulfilled.",
            Budget = 5000,
            NumberOfParticipants = 2
        };

        // Act
        var actionResult = controller.Update(-4, dto);
        var result = actionResult.Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Update_fails_for_different_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerForDifferentTourist(scope);

        var dto = new UpdateTourRequestDto
        {
            Id = -1, // Request belongs to tourist -21
            Title = "Pokušaj izmene tuđeg zahteva",
            Description = "Ovaj pokušaj izmene bi trebalo da ne uspe jer request ne pripada turistu.",
            Budget = 5000,
            NumberOfParticipants = 2
        };

        // Act
        var actionResult = controller.Update(-1, dto);

        // Assert - trebalo bi da vrati Forbid (403)
        actionResult.Result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Closes_tour_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var actionResult = controller.Close(-2);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-2);
        result.Status.ShouldBe(3); // Closed

        // Assert - Database
        var stored = dbContext.TourRequests.Find((long)-2);
        stored.ShouldNotBeNull();
        stored.Status.ShouldBe(Explorer.Tours.Core.Domain.TourRequestStatus.Closed);
    }

    [Fact]
    public void Deletes_open_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Prvo kreiraj request pa ga obriši
        var createDto = new CreateTourRequestDto
        {
            Title = "Request za brisanje",
            Description = "Ovaj request će biti obrisan u testu kako bi se testirala delete funkcionalnost.",
            Budget = 5000
        };

        var createResult = controller.Create(createDto);
        var created = ((ObjectResult)createResult.Result)?.Value as TourRequestDto;
        created.ShouldNotBeNull();

        // Act - Delete
        var deleteResult = controller.Delete(created.Id);

        // Assert
        deleteResult.ShouldBeOfType<NoContentResult>();

        // Assert - Database
        var deleted = dbContext.TourRequests.FirstOrDefault(tr => tr.Id == created.Id);
        deleted.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_for_fulfilled_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // Owner je -22
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Proveri da li -4 postoji i da li je Fulfilled
        var request = dbContext.TourRequests.Find((long)-4);
        request.ShouldNotBeNull("Request -4 should exist in test data");
        request.Status.ShouldBe(Explorer.Tours.Core.Domain.TourRequestStatus.Fulfilled, "Request -4 should be Fulfilled");

        // Act
        var actionResult = controller.Delete(-4); // Fulfilled request
        var result = actionResult as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Accepts_response_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // Owner of request -3
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Prvo proveri da li request -3 postoji i da li ima responses
        var requestBefore = dbContext.TourRequests.Find((long)-3);
        requestBefore.ShouldNotBeNull("Request -3 should exist");

        var response4 = dbContext.TourRequestResponses.Find((long)-4);
        response4.ShouldNotBeNull("Response -4 should exist");

        var response5 = dbContext.TourRequestResponses.Find((long)-5);
        response5.ShouldNotBeNull("Response -5 should exist");

        var dto = new AcceptResponseDto
        {
            TourRequestId = -3,
            ResponseId = -4
        };

        // Act
        var actionResult = controller.AcceptResponse(dto);

        // Debug
        var okResult = actionResult.Result as OkObjectResult;
        var badResult = actionResult.Result as BadRequestObjectResult;

        actionResult.Result.ShouldNotBeNull($"Result should not be null. Got type: {actionResult.Result?.GetType().Name}");

        if (badResult != null)
        {
            throw new Exception($"Got BadRequest instead of Ok: {badResult.Value}");
        }

        var result = okResult?.Value as TourRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-3);
        result.Status.ShouldBe(2); // Fulfilled

        // Detach entities to force fresh load
        dbContext.ChangeTracker.Clear();

        // Refresh entities from database
        var request = dbContext.TourRequests.Find((long)-3);
        var acceptedResponse = dbContext.TourRequestResponses.Find((long)-4);
        var rejectedResponse = dbContext.TourRequestResponses.Find((long)-5);

        // Assert - Database
        request.ShouldNotBeNull();
        request.Status.ShouldBe(Explorer.Tours.Core.Domain.TourRequestStatus.Fulfilled);

        acceptedResponse.ShouldNotBeNull();
        acceptedResponse.Status.ShouldBe(Explorer.Tours.Core.Domain.ResponseStatus.Accepted);

        rejectedResponse.ShouldNotBeNull();
        rejectedResponse.Status.ShouldBe(Explorer.Tours.Core.Domain.ResponseStatus.Rejected);
    }

    private static TourRequestController CreateController(IServiceScope scope, string touristId = "-21")
    {
        return new TourRequestController(scope.ServiceProvider.GetRequiredService<ITourRequestService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }

    private static TourRequestController CreateControllerForDifferentTourist(IServiceScope scope)
    {
        return CreateController(scope, "-99"); // Different tourist
    }
}