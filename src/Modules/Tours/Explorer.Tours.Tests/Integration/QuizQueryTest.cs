using AutoMapper;
using Explorer.API.Controllers;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class QuizQueryTests : BaseToursIntegrationTest
{
    public QuizQueryTests(ToursTestFactory factory) : base(factory) { }

    //get by id
    [Fact]
    public void Retrieves_by_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "1"); // user ID = 1

        var createDto = new CreateQuizDto
        {
            Title = "Test Quiz",
        };

        var createdResult = controller.Create(createDto).Result as OkObjectResult;
        createdResult.ShouldNotBeNull();

        var createdQuiz = createdResult.Value as QuizDto;
        createdQuiz.ShouldNotBeNull();
        // createdQuiz.Id.ShouldBeGreaterThan(0);
        createdQuiz.AuthorId.ShouldBe(1);


        // 2) Act
        var getResult = controller.Get(createdQuiz.Id).Result as OkObjectResult;
        getResult.ShouldNotBeNull();

        var quiz = getResult.Value as QuizDto;

        // 3) Assert
        quiz.ShouldNotBeNull();
        quiz.Title.ShouldBe("Test Quiz");
        quiz.Id.ShouldBe(createdQuiz.Id);
        quiz.AuthorId.ShouldBe(1);
    }

    // get by author id
    [Fact]
    public void Retrieves_by_author()
    {
        using var scope = Factory.Services.CreateScope();
        long authorId = 5;

        var controller = CreateController(scope, authorId.ToString()); 

        var createDto = new CreateQuizDto
        {
            Title = "Author Test Quiz",
        };

        var createdResult = controller.Create(createDto).Result as OkObjectResult;
        createdResult.ShouldNotBeNull();

        var createdQuiz = createdResult.Value as QuizDto;
        createdQuiz.ShouldNotBeNull();
        createdQuiz.AuthorId.ShouldBe(authorId);

        // Act 
        var getResult = controller.GetByAuthor(authorId).Result as OkObjectResult;
        getResult.ShouldNotBeNull();

        var quizzes = getResult.Value as List<QuizDto>;

        // Assert
        quizzes.ShouldNotBeNull();
        quizzes.Count.ShouldBeGreaterThan(0);
        quizzes.Any(q => q.Id == createdQuiz.Id).ShouldBeTrue();
    }


    private static QuizController CreateController(IServiceScope scope)
    {
        return new QuizController(
            scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext("1")
        };
    }

    private static QuizController CreateController(IServiceScope scope, string userId)
    {
        return new QuizController(
            scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext(userId)
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
