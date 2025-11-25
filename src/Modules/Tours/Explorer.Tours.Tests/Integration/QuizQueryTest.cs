using AutoMapper;
using Explorer.API.Controllers;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class QuizQueryTests : BaseToursIntegrationTest
{
    public QuizQueryTests(ToursTestFactory factory) : base(factory) { }

    //get by id
    [Fact]
    public void Retrieves_by_id()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        //act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as QuizDto;


        // assert
        result.ShouldNotBeNull();
        result.Title.ShouldNotBeNullOrEmpty();
    }

    // get by author id
    [Fact]
    public void Retrieves_by_author()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetByAuthor(-11).Result)?.Value as List<QuizDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    private static QuizController CreateController(IServiceScope scope)
    {
        return new QuizController(
            scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
