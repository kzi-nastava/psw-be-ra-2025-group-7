using Explorer.API.Controllers.Tourist.Notes;
using Explorer.Notes.API.Dtos;
using Explorer.Notes.API.Public;
using Explorer.Notes.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Explorer.Notes.Tests.Integration
{
    [Collection("Sequential")]
    public class NoteIntegrationTests : BaseNotesIntegrationTest
    {
        public NoteIntegrationTests(NotesTestFactory factory) : base(factory) { }

        [Fact]
        public void Get_my_notes_returns_only_current_user_notes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            // Act
            var actionResult = controller.GetMyNotes();
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as List<NoteDto>;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldAllBe(n => n.UserId == -11);
        }

        [Fact]
        public void Creates_note_and_persists_in_database()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NotesContext>();

            const long userId = 1; // Pozitivan ID za kreiranje
            var controller = CreateController(scope, userId.ToString());

            var dto = new CreateNoteDto
            {
                Title = "Test Note",
                Content = "This is a test note content.",
                Type = API.Dtos.NoteTypeDto.Plan,
                Tags = new List<string> { "Test", "Integration" }
            };

            // Act
            var actionResult = controller.Create(dto);
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as NoteDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Title.ShouldBe(dto.Title);
            result.Content.ShouldBe(dto.Content);
            result.UserId.ShouldBe(userId);
            result.Tags.Count.ShouldBe(2);

            // Assert - Database
            var stored = dbContext.Notes.FirstOrDefault(n => n.Id == result.Id);
            stored.ShouldNotBeNull();
            stored.Title.ShouldBe(dto.Title);
        }

        [Fact]
        public void Updates_note_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var updateDto = new UpdateNoteDto
            {
                Id = -1,
                Title = "Updated Weekend Trip",
                Content = "Updated content with new plans",
                Type = API.Dtos.NoteTypeDto.Plan,
                Tags = new List<string> { "Updated", "Weekend" }
            };

            // Act
            var actionResult = controller.Update(-1, updateDto);
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as NoteDto;

            // Assert
            result.ShouldNotBeNull();
            result.Title.ShouldBe(updateDto.Title);
            result.Content.ShouldBe(updateDto.Content);
        }

        [Fact]
        public void Deletes_note_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NotesContext>();
            var controller = CreateController(scope, "-11");

            // Act
            var actionResult = controller.Delete(-4);

            // Assert
            actionResult.ShouldBeOfType<OkObjectResult>();

            var deleted = dbContext.Notes.FirstOrDefault(n => n.Id == -4);
            deleted.ShouldBeNull();
        }

        [Fact]
        public void Pinned_notes_appear_first()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            // Act
            var actionResult = controller.GetMyNotes();
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as List<NoteDto>;

            // Assert
            result.ShouldNotBeNull();
            result.First().IsPinned.ShouldBeTrue();
            result.First().Id.ShouldBe(-2); // Pinned note
        }

        private static NoteController CreateController(IServiceScope scope, string userId)
        {
            var controller = new NoteController(
                scope.ServiceProvider.GetRequiredService<INoteService>());

            var ctx = BuildContext(userId);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim("id", userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "tourist")
            }, "test");

            ctx.HttpContext.User = new ClaimsPrincipal(identity);
            controller.ControllerContext = ctx;
            return controller;
        }
    }
}