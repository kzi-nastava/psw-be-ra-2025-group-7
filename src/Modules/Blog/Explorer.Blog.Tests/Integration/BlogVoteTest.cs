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
            blogPost.Score.ShouldBe(0); // inicijalno score = 0

            // 2) Glasaj +1
            var voteDto = controller.Vote(blogPost.Id, 1).Result as OkObjectResult;
            voteDto.ShouldNotBeNull();
            var vote1 = voteDto.Value as BlogVoteDto;
            vote1.ShouldNotBeNull();
            vote1.Value.ShouldBe(1);

            // Reload post iz baze i proveri score
            var storedPost = dbContext.BlogPosts.First(p => p.Id == blogPost.Id);
            storedPost.Score.ShouldBe(1);

            // 3) Promeni glas na -1
            var vote2Dto = controller.Vote(blogPost.Id, -1).Result as OkObjectResult;
            vote2Dto.ShouldNotBeNull();
            var vote2 = vote2Dto.Value as BlogVoteDto;
            vote2.ShouldNotBeNull();
            vote2.Value.ShouldBe(-1);

            storedPost = dbContext.BlogPosts.First(p => p.Id == blogPost.Id);
            storedPost.Score.ShouldBe(-1);

            // 4) Povuci glas (-1)
            var vote3Dto = controller.Vote(blogPost.Id, -1).Result as OkObjectResult;
            vote3Dto.ShouldNotBeNull();
            var vote3 = vote3Dto.Value as BlogVoteDto;
            vote3.ShouldBeNull(); // glas je uklonjen

            storedPost = dbContext.BlogPosts.First(p => p.Id == blogPost.Id);
            storedPost.Score.ShouldBe(0);
        }

        private static BlogPostController CreateController(IServiceScope scope, string userId)
        {
            var controller = new BlogPostController(
                scope.ServiceProvider.GetRequiredService<IBlogPostService>());

            var ctx = BuildContext(userId);

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

        private static ControllerContext BuildContext(string userId)
        {
            return new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
            };
        }


        [Fact]
        public void MultipleUsersVoting_Should_AggregateScoreCorrectly()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            // User 1
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

            // 2) User 1 glasa +1
            var v1 = controllerUser1.Vote(post.Id, 1).Result as OkObjectResult;
            v1.ShouldNotBeNull();

            // User 2 glasa -1
            var controllerUser2 = CreateController(scope, "2");
            var v2 = controllerUser2.Vote(post.Id, -1).Result as OkObjectResult;
            v2.ShouldNotBeNull();

            // User 3 glasa +1
            var controllerUser3 = CreateController(scope, "3");
            var v3 = controllerUser3.Vote(post.Id, 1).Result as OkObjectResult;
            v3.ShouldNotBeNull();

            // Reload
            var stored = dbContext.BlogPosts.First(x => x.Id == post.Id);

            // Score: user1(+1) + user2(-1) + user3(+1) = +1
            stored.Score.ShouldBe(1);

            // User 2 promeni glas na +1
            var v2Changed = controllerUser2.Vote(post.Id, 1).Result as OkObjectResult;
            v2Changed.ShouldNotBeNull();

            stored = dbContext.BlogPosts.First(x => x.Id == post.Id);

            // Novi score: user1(+1) + user2(+1) + user3(+1) = 3
            stored.Score.ShouldBe(3);
        }

    }
}