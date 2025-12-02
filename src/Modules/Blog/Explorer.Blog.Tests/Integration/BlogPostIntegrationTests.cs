using Explorer.API.Controllers;
using Explorer.API.Controllers.Tourist.Blog;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Xunit;

namespace Explorer.Blog.Tests.Integration;


[Collection("Sequential")]
public class BlogPostIntegrationTests : BaseBlogIntegrationTest
{
    public BlogPostIntegrationTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_my_blog_posts_returns_only_current_author_posts()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
       
        var controller = CreateController(scope, "-11");

        // Act
        var actionResult = controller.GetMyBlogPosts(1, 10);
        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult?.Value as PagedResult<BlogPostDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThan(0);

        
        foreach (var post in result.Results)
        {
            post.AuthorId.ShouldBe(-11);  // svi vraćeni postovi moraju da pripadaju ulogovanom autoru
        }
    }

    [Fact]
    public void Creates_blog_post_for_current_user_and_persists_in_database()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        
        const long authorId = 1;   //za kreiranje treba pozitivan ID
        var controller = CreateController(scope, authorId.ToString());

        var dto = new CreateBlogPostDto
        {
            Title = "Novi blog post (integration test)",
            Description = "Opis novog blog posta iz integracionog testa.",
            Images = new List<BlogImageDto>
            {
                new() { Url = "http://example.com/first.jpg",  Order = 0 },
                new() { Url = "http://example.com/second.jpg", Order = 1 }
            }
        };

        // Act
        var actionResult = controller.Create(dto);
        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult?.Value as BlogPostDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(dto.Title);
        result.Description.ShouldBe(dto.Description);
        result.AuthorId.ShouldBe(authorId);
        result.Images.Count.ShouldBe(2);

        // Assert
        var stored = dbContext.BlogPosts.FirstOrDefault(p => p.Id == result.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Description.ShouldBe(dto.Description);
        stored.AuthorId.ShouldBe(authorId);
        stored.Images.Count.ShouldBe(2);
        stored.Images
            .OrderBy(i => i.Order)
            .Select(i => i.Url)
            .ToList()
            .ShouldBe(dto.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList());
    }

    [Fact]
    public void Updates_draft_blog_post_for_current_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        // Uzimamo postojeći blog (iz seed-a) – pretpostavljamo da je njegov autor -11
        var existing = dbContext.BlogPosts.First(p => p.Id == -1);

        var updateDto = new UpdateBlogPostDto
        {
            Id = existing.Id,
            Title = existing.Title + " (izmenjeno)",
            Description = "Novi opis posle izmene.",
            Images = new List<BlogImageDto>
        {
            new() { Url = "http://example.com/updated1.jpg", Order = 0 },
            new() { Url = "http://example.com/updated2.jpg", Order = 1 }
        }
        };

        // Act – sada koristimo NOVU metodu iz kontrolera
        var actionResult = controller.UpdateDraft(updateDto);
        var okResult = actionResult.Result as OkObjectResult;
        var updated = okResult?.Value as BlogPostDto;

        // Assert - Response
        updated.ShouldNotBeNull();
        updated.Id.ShouldBe(existing.Id);
        updated.Title.ShouldBe(updateDto.Title);
        updated.Description.ShouldBe(updateDto.Description);
        updated.AuthorId.ShouldBe(-11);
        updated.Images.Count.ShouldBe(2);

        // Assert - Database
        var stored = dbContext.BlogPosts.FirstOrDefault(p => p.Id == existing.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(updateDto.Title);
        stored.Description.ShouldBe(updateDto.Description);
        stored.Images.Count.ShouldBe(2);
        stored.Images
            .OrderBy(i => i.Order)
            .Select(i => i.Url)
            .ToList()
            .ShouldBe(updateDto.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList());
    }



    private static BlogPostController CreateController(IServiceScope scope, string userId)
    {
        var controller = new BlogPostController(
            scope.ServiceProvider.GetRequiredService<IBlogPostService>());

       
        var ctx = BuildContext(userId);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim("id", userId),                      // ono što traži GetCurrentUserId()
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "author")          // da prođe [Authorize(Roles="tourist,author")]
        }, "test");

        ctx.HttpContext.User = new ClaimsPrincipal(identity);
        controller.ControllerContext = ctx;
        return controller;
    }
}