using Explorer.API.Controllers.Author.Tours;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class AuthorTourRequestCommandTests : BaseToursIntegrationTest
{
    public AuthorTourRequestCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_existing_tour_response_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestResponseDto
        {
            TourRequestId = -6,     // open request without response
            ResponseType = 0,       // ExistingTour
            TourId = -3,            // published tour
            ProposedPrice = 20000
        };

        var actionResult = controller.CreateResponse(-6, dto);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestResponseDto;

        result.ShouldNotBeNull();
        result.TourRequestId.ShouldBe(-6);
        result.ResponseType.ShouldBe(0);
        result.TourId.ShouldBe(-3);
        result.Status.ShouldBe(0); // Pending
    }

    [Fact]
    public void Create_existing_tour_fails_with_draft_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestResponseDto
        {
            TourRequestId = -6,
            ResponseType = 0,
            TourId = -1, // draft tour
            ProposedPrice = 20000
        };

        var actionResult = controller.CreateResponse(-6, dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_custom_proposal_fails_when_tour_id_is_set()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestResponseDto
        {
            TourRequestId = -6,
            ResponseType = 1,   // CustomProposal
            TourId = -3,        
            ProposedPrice = 20000
        };

        var actionResult = controller.CreateResponse(-6, dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_fails_for_duplicate_response()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestResponseDto
        {
            TourRequestId = -1, // already has response
            ResponseType = 0,
            TourId = -3,
            ProposedPrice = 15000
        };

        var actionResult = controller.CreateResponse(-1, dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_fails_for_closed_request()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourRequestResponseDto
        {
            TourRequestId = -5, // closed request
            ResponseType = 0,
            TourId = -3,
            ProposedPrice = 15000
        };

        var actionResult = controller.CreateResponse(-5, dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }


    [Fact]
    public void Updates_response_price_and_message()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new UpdateMyResponseDto
        {
            ProposedPrice = 16000,
            Message = "Updated message"
        };

        var actionResult = controller.UpdateMyResponse(-1, dto);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourRequestResponseDto;

        result.ShouldNotBeNull();
        result.ProposedPrice.ShouldBe(16000);
        result.Message.ShouldBe("Updated message");
    }

    [Fact]
    public void Update_fails_for_accepted_response()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new UpdateMyResponseDto { ProposedPrice = 5000 };

        var actionResult = controller.UpdateMyResponse(-6, dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Update_fails_for_rejected_response()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new UpdateMyResponseDto { ProposedPrice = 5000 };

        var actionResult = controller.UpdateMyResponse(-7, dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }


    [Fact]
    public void Delete_fails_for_accepted_response()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var actionResult = controller.DeleteMyResponse(-6);

        actionResult.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Delete_fails_for_different_author()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-2");

        var actionResult = controller.DeleteMyResponse(-1);

        actionResult.ShouldBeOfType<ForbidResult>();
    }


    private static AuthorTourRequestController CreateController(
        IServiceScope scope, string authorId = "-1")
    {
        return new AuthorTourRequestController(
            scope.ServiceProvider.GetRequiredService<IAuthorTourRequestService>())
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}
