using Explorer.API.Controllers;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class QuizCommandTests : BaseToursIntegrationTest
{
    public QuizCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new CreateQuizDto
        {
           // AuthorId = 1,
            Title = "Osnove geografije",
            Questions = new List<CreateQuestionDto>
            {
                new CreateQuestionDto
                {
                    Content = "Koja je glavna reka u Egiptu?",
                    AllowsMultipleCorrect = false,
                    Options = new List<CreateOptionDto>
                    {
                        new CreateOptionDto { Text = "Nil", IsCorrect = true, Feedback = "Nil je najduža reka u Africi." },
                        new CreateOptionDto { Text = "Amazon", IsCorrect = false, Feedback = "Amazon je u Južnoj Americi." },
                        new CreateOptionDto { Text = "Dunav", IsCorrect = false, Feedback = "Dunav protiče kroz Evropu." },
                        new CreateOptionDto { Text = "Seina", IsCorrect = false, Feedback = "Seina protiče kroz Francusku." }
                    }
                }
            }
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as QuizDto;


        // Assert - Response
        result.ShouldNotBeNull();
        result.Title.ShouldBe(newEntity.Title);
        result.Questions.ShouldNotBeNull();
        result.Questions.Count.ShouldBe(1);

        // Assert - Database
        var storedEntity = dbContext.Quizzes.FirstOrDefault(i => i.Title == newEntity.Title);
        storedEntity.ShouldNotBeNull();
        storedEntity.Title.ShouldBe(newEntity.Title);
        storedEntity.AuthorId.ShouldBe(1);
    }

    private static QuizController CreateController(IServiceScope scope)
    {
        return new QuizController(scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext("1")
        };
    }

    private static ControllerContext BuildContext(string userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id", userId)
        }, "mock"));

        return new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = user
            }
        };
    }


}
