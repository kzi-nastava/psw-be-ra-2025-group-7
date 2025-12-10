using Explorer.API.Controllers.Tourist.Blog;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Explorer.Blog.Tests.Integration
{
    [Collection("Sequential")]
    public class BlogVoteIntegrationTests : BaseBlogIntegrationTest
    {
        public BlogVoteIntegrationTests(BlogTestFactory factory) : base(factory) { }

        [Fact]
        public void Vote_Should_UpdateScoreAndPersistVote()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
            var controller = CreateController(scope, "1"); // user ID = 1

            // 1) Kreiraj blog post
            var createDto = new CreateBlogPostDto
            {
                Title = "Blog za glasanje",
                Description = "Opis",
                Images = new List<BlogImageDto>()
            };

            var createdResult = controller.Create(createDto).Result as OkObjectResult;
            createdResult.ShouldNotBeNull();

            var blogPost = createdResult.Value as BlogPostDto;
            blogPost.ShouldNotBeNull();
            blogPost.Score.ShouldBe(0);

            // 2) Glasaj +1
            var vote = new BlogVoteDto { Value = 1 };
            var result = controller.Vote(blogPost.Id, vote).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var updatedPost = result.Value as BlogPostDto;
            updatedPost.ShouldNotBeNull();
            updatedPost.Score.ShouldBe(1);

            // Provera u bazi
            var storedPost = dbContext.BlogPosts.First(p => p.Id == blogPost.Id);
            storedPost.Score.ShouldBe(1);

            // 3) Promeni glas na -1
            var vote2 = new BlogVoteDto { Value = -1 };
            var result2 = controller.Vote(blogPost.Id, vote2).Result as OkObjectResult;
            result2.ShouldNotBeNull();

            var updatedPost2 = result2.Value as BlogPostDto;
            updatedPost2.ShouldNotBeNull();
            updatedPost2.Score.ShouldBe(-1);

            storedPost = dbContext.BlogPosts.First(p => p.Id == blogPost.Id);
            storedPost.Score.ShouldBe(-1);

            // 4) Povuci glas
            var vote3 = new BlogVoteDto { Value = -1 };
            var result3 = controller.Vote(blogPost.Id, vote3).Result as OkObjectResult;
            result3.ShouldNotBeNull();

            var updatedPost3 = result3.Value as BlogPostDto;
            updatedPost3.ShouldNotBeNull();
            updatedPost3.Score.ShouldBe(0);

            storedPost = dbContext.BlogPosts.First(p => p.Id == blogPost.Id);
            storedPost.Score.ShouldBe(0);
        }

        private static BlogPostController CreateController(IServiceScope scope, string userId)
        {
            var controller = new BlogPostController(
                scope.ServiceProvider.GetRequiredService<IBlogPostService>());

            var ctx = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
            };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "author")
            }, "test");

            ctx.HttpContext.User = new ClaimsPrincipal(identity);
            controller.ControllerContext = ctx;
            return controller;
        }

        [Fact]
        public void MultipleUsersVoting_Should_AggregateScoreCorrectly()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var controllerUser1 = CreateController(scope, "1");

            // 1) User 1 kreira blog
            var createDto = new CreateBlogPostDto
            {
                Title = "Multi user glasanje",
                Description = "Opis",
                Images = new List<BlogImageDto>()
            };

            var createdResult = controllerUser1.Create(createDto).Result as OkObjectResult;
            createdResult.ShouldNotBeNull();
            var post = createdResult.Value as BlogPostDto;
            post.ShouldNotBeNull();
            post.Score.ShouldBe(0);

            // User 1 glasa +1
            var v11 = new BlogVoteDto { Value = 1 };
            controllerUser1.Vote(post.Id, v11).Result.ShouldBeOfType<OkObjectResult>();

            // User 2 glasa -1
            var controllerUser2 = CreateController(scope, "2");
            var v22 = new BlogVoteDto { Value = -1 };
            controllerUser2.Vote(post.Id, v22).Result.ShouldBeOfType<OkObjectResult>();

            // User 3 glasa +1
            var controllerUser3 = CreateController(scope, "3");
            var v33 = new BlogVoteDto { Value = 1 };
            controllerUser3.Vote(post.Id, v33).Result.ShouldBeOfType<OkObjectResult>();

            // Score = +1 -1 +1 = +1
            var stored = dbContext.BlogPosts.First(x => x.Id == post.Id);
            stored.Score.ShouldBe(1);

            // User 2 promeni glas na +1
            var vt = new BlogVoteDto { Value = 1 };
            controllerUser2.Vote(post.Id, vt).Result.ShouldBeOfType<OkObjectResult>();

            stored = dbContext.BlogPosts.First(x => x.Id == post.Id);

            // Novi score = +1 +1 +1 = 3
            stored.Score.ShouldBe(3);
        }
    }
}
