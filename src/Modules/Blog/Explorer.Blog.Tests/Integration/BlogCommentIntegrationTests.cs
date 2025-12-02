using Explorer.API.Controllers.Tourist.Blog;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class BlogCommentIntegrationTests : BaseBlogIntegrationTest
{
    public BlogCommentIntegrationTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_comment_for_published_blog_and_persists_in_database()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        const long authorId = -12;
        var controller = CreateCommentController(scope, authorId.ToString());

        // Pretpostavljamo da postoji Published blog sa ID = -2
        var blog = dbContext.BlogPosts
            .Include(b => b.Comments)
            .First(p => p.Id == -2);

        var initialCommentCount = blog.Comments.Count;

        var dto = new CreateCommentDto
        {
            BlogId = -2,
            Text = "Ovo je testni komentar iz integration testa."
        };

        // Act
        var actionResult = controller.Create(dto);
        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult?.Value as BlogCommentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.BlogId.ShouldBe(dto.BlogId);
        result.AuthorId.ShouldBe(authorId);
        result.Text.ShouldBe(dto.Text);
        result.CreatedAt.ShouldNotBe(default);
        result.LastModifiedAt.ShouldBeNull();

        // Assert - Database
        var updatedBlog = dbContext.BlogPosts
            .Include(b => b.Comments)
            .First(p => p.Id == -2);

        updatedBlog.Comments.Count.ShouldBe(initialCommentCount + 1);

        var storedComment = updatedBlog.Comments.FirstOrDefault(c => c.Id == result.Id);
        storedComment.ShouldNotBeNull();
        storedComment.Text.ShouldBe(dto.Text);
        storedComment.AuthorId.ShouldBe(authorId);
    }

    [Fact]
    public void Cannot_create_comment_on_draft_blog()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        const long authorId = -12;
        var controller = CreateCommentController(scope, authorId.ToString());

        // Pretpostavljamo da postoji Draft blog sa ID = -1
        var dto = new CreateCommentDto
        {
            BlogId = -1,  // Draft blog
            Text = "Pokušaj komentarisanja Draft bloga."
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Create(dto);
        }).Message.ShouldContain("Comments can only be added to published blogs");
    }

    [Fact]
    public void Blog_status_changes_to_active_after_5_comments()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        // Kreiraj novi Published blog
        var newBlog = new Core.Domain.BlogPost(-11, "Test Blog za Active Status", "Opis");
        newBlog.Publish();
        dbContext.BlogPosts.Add(newBlog);
        dbContext.SaveChanges();

        var controller = CreateCommentController(scope, "-11");

        // Act - Dodaj 5 komentara
        for (int i = 1; i <= 5; i++)
        {
            var dto = new CreateCommentDto
            {
                BlogId = newBlog.Id,
                Text = $"Komentar broj {i}"
            };
            controller.Create(dto);
        }

        // Assert
        var updatedBlog = dbContext.BlogPosts
            .Include(b => b.Comments)
            .First(b => b.Id == newBlog.Id);

        updatedBlog.Comments.Count.ShouldBe(5);
        updatedBlog.Status.ShouldBe(Core.Domain.BlogStatus.Active);
    }

    [Fact]
    public void Blog_status_changes_to_famous_after_10_comments()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        // Kreiraj novi Published blog
        var newBlog = new Core.Domain.BlogPost(-12, "Test Blog za Famous Status", "Opis");
        newBlog.Publish(); 
        dbContext.BlogPosts.Add(newBlog);
        dbContext.SaveChanges();
        newBlog.Status.ShouldBe(Core.Domain.BlogStatus.Published);

        var controller = CreateCommentController(scope, "-12");

        // Act - Dodaj 10 komentara
        for (int i = 1; i <= 12; i++)
        {
            var dto = new CreateCommentDto
            {
                BlogId = newBlog.Id,
                Text = $"Komentar {i}"
            };
            controller.Create(dto);
        }
        
        // Assert
        var updatedBlog = dbContext.BlogPosts
            .Include(b => b.Comments)
            .First(b => b.Id == newBlog.Id);

        updatedBlog.Comments.Count.ShouldBe(12);
        updatedBlog.Status.ShouldBe(Core.Domain.BlogStatus.Famous);
    }

    private static BlogCommentController CreateCommentController(IServiceScope scope, string userId)
    {
        var controller = new BlogCommentController(
            scope.ServiceProvider.GetRequiredService<IBlogCommentService>());

        var ctx = BuildContext(userId);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim("id", userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "tourist")  // može biti bilo koja uloga
        }, "test");

        ctx.HttpContext.User = new ClaimsPrincipal(identity);
        controller.ControllerContext = ctx;
        return controller;
    }
}