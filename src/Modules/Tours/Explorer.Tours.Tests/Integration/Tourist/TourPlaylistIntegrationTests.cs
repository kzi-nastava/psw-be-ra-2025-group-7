using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPlaylistIntegrationTests : BaseToursIntegrationTest, IAsyncLifetime
{
    public TourPlaylistIntegrationTests(ToursTestFactory factory) : base(factory) { }

    public async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await CreatePersonsTableIfNotExists(dbContext);

        // Cleanup conflicts from seed data
        // 1. First delete TourReviews (has FK to TourExecutions)
        await dbContext.Database.ExecuteSqlRawAsync(@"
        DELETE FROM tours.""TourReviews"" 
        WHERE ""TourExecutionId"" IN (
            SELECT ""Id"" FROM tours.""TourExecutions""
            WHERE ""TouristId"" IN (-3, -21) 
            OR ""TourId"" = -3
        );
    ");

        // 2. Now delete TourExecutions
        await dbContext.Database.ExecuteSqlRawAsync(@"
        DELETE FROM tours.""TourExecutions"" 
        WHERE ""TouristId"" IN (-3, -21) 
        OR ""TourId"" = -3;
    ");
    }

    public async Task DisposeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Cleanup test data
        await CleanupPersons(dbContext);

        // OBRIŠI CELU stakeholders šemu DA BI Stakeholders testovi mogli da je kreiraju ponovo
        await dbContext.Database.ExecuteSqlRawAsync(@"
        DROP SCHEMA IF EXISTS stakeholders CASCADE;
    ");
    }

    [Fact]
    public async Task GeneratePlaylist_Creates_Playlist_Successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop", "indie" },
            IncludeWeather = false
        };

        // Act
        var result = await controller.GeneratePlaylist(execution.Id, dto);

        // Assert
        result.ShouldNotBeNull();
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var playlist = okResult.Value as TourPlaylistDto;
        playlist.ShouldNotBeNull();
        playlist.TourExecutionId.ShouldBe(execution.Id);
        playlist.SelectedGenres.ShouldContain("pop");
        playlist.SelectedGenres.ShouldContain("indie");
        playlist.Tracks.ShouldNotBeEmpty();
        playlist.TotalDurationMinutes.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GeneratePlaylist_With_Weather_Includes_Weather_Data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop" },
            IncludeWeather = true
        };

        // Act
        var result = await controller.GeneratePlaylist(execution.Id, dto);

        // Assert
        result.ShouldNotBeNull();
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var playlist = okResult.Value as TourPlaylistDto;
        playlist.ShouldNotBeNull();
        playlist.IncludeWeather.ShouldBeTrue();
    }

    [Fact]
    public async Task GeneratePlaylist_Fails_With_No_Genres()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string>(),
            IncludeWeather = false
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await controller.GeneratePlaylist(execution.Id, dto));
    }

    [Fact]
    public async Task GeneratePlaylist_Fails_With_Too_Many_Genres()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop", "rock", "jazz", "indie", "electronic", "hip-hop" },
            IncludeWeather = false
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await controller.GeneratePlaylist(execution.Id, dto));
    }

    [Fact]
    public async Task GeneratePlaylist_Replaces_Existing_Playlist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        // Generate first playlist
        var firstDto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop" },
            IncludeWeather = false
        };
        await controller.GeneratePlaylist(execution.Id, firstDto);

        // Act - Generate second playlist with different genres
        var secondDto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "rock", "metal" },
            IncludeWeather = false
        };
        var result = await controller.GeneratePlaylist(execution.Id, secondDto);

        // Assert
        result.ShouldNotBeNull();
        var okResult = result.Result as OkObjectResult;
        var playlist = okResult.Value as TourPlaylistDto;

        playlist.SelectedGenres.ShouldContain("rock");
        playlist.SelectedGenres.ShouldContain("metal");
        playlist.SelectedGenres.ShouldNotContain("pop");

        // Verify only one playlist exists for this execution
        var playlists = dbContext.TourPlaylists.Where(p => p.TourExecutionId == execution.Id).ToList();
        playlists.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetPlaylist_Returns_Existing_Playlist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        // Create playlist first
        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop", "indie" },
            IncludeWeather = false
        };
        await controller.GeneratePlaylist(execution.Id, dto);

        // Act
        var result = controller.GetPlaylist(execution.Id);

        // Assert
        result.ShouldNotBeNull();
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var playlist = okResult.Value as TourPlaylistDto;
        playlist.ShouldNotBeNull();
        playlist.TourExecutionId.ShouldBe(execution.Id);
        playlist.Tracks.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetPlaylist_Throws_When_Not_Found()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        // Act & Assert - No playlist created for this execution
        Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.NotFoundException>(() =>
            controller.GetPlaylist(execution.Id));
    }

    [Fact]
    public async Task GetPlaylist_Throws_When_Not_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        // User -3 creates execution and playlist
        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        var playlistService = scope.ServiceProvider.GetRequiredService<ITourPlaylistService>();
        await playlistService.GeneratePlaylist(-3, execution.Id, new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop" },
            IncludeWeather = false
        });

        // User -2 tries to access it
        var controller = CreateController(scope, "-2");

        // Act & Assert
        Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.ForbiddenException>(() =>
            controller.GetPlaylist(execution.Id));
    }

    [Fact]
    public async Task DeletePlaylist_Removes_Playlist_Successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        // Create playlist
        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop" },
            IncludeWeather = false
        };
        await controller.GeneratePlaylist(execution.Id, dto);

        // Verify playlist exists
        var existingPlaylist = dbContext.TourPlaylists.FirstOrDefault(p => p.TourExecutionId == execution.Id);
        existingPlaylist.ShouldNotBeNull();

        // Act
        var result = controller.DeletePlaylist(execution.Id);

        // Assert
        result.ShouldNotBeNull();
        var noContentResult = result as NoContentResult;
        noContentResult.ShouldNotBeNull();

        // Verify playlist is deleted
        var deletedPlaylist = dbContext.TourPlaylists.FirstOrDefault(p => p.TourExecutionId == execution.Id);
        deletedPlaylist.ShouldBeNull();
    }

    [Fact]
    public async Task DeletePlaylist_Throws_When_Not_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsurePersonsExist(dbContext);
        await EnsureTourHasDuration(dbContext, -3);
        await EnsurePurchaseToken(dbContext, -3, -3);

        // User -3 creates execution and playlist
        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        var playlistService = scope.ServiceProvider.GetRequiredService<ITourPlaylistService>();
        await playlistService.GeneratePlaylist(-3, execution.Id, new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop" },
            IncludeWeather = false
        });

        // User -2 tries to delete it
        var controller = CreateController(scope, "-2");

        // Act & Assert
        Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.ForbiddenException>(() =>
            controller.DeletePlaylist(execution.Id));
    }

    // ==================== HELPER METHODS ====================

    private async Task CreatePersonsTableIfNotExists(ToursContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE SCHEMA IF NOT EXISTS stakeholders;
            
            CREATE TABLE IF NOT EXISTS stakeholders.""Users"" (
                ""Id"" bigint NOT NULL,
                ""Username"" text NOT NULL,
                ""Password"" text NOT NULL,
                ""Role"" integer NOT NULL,
                ""IsActive"" boolean NOT NULL DEFAULT true,
                CONSTRAINT ""PK_Users"" PRIMARY KEY (""Id"")
            );
            
            CREATE TABLE IF NOT EXISTS stakeholders.""People"" (
                ""Id"" bigint NOT NULL,
                ""UserId"" bigint NOT NULL,
                ""Name"" text NOT NULL,
                ""Surname"" text NOT NULL,
                ""Email"" text NOT NULL,
                CONSTRAINT ""PK_People"" PRIMARY KEY (""Id""),
                CONSTRAINT ""FK_People_Users_UserId"" FOREIGN KEY (""UserId"")
                    REFERENCES stakeholders.""Users"" (""Id"") ON DELETE CASCADE
            );
        ");
    }

    private async Task EnsurePersonsExist(ToursContext dbContext)
    {
        // 1. Kreiraj Users
        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO stakeholders.""Users"" (""Id"", ""Username"", ""Password"", ""Role"", ""IsActive"")
            VALUES 
                (-1, 'author1', 'password', 1, true),
                (-2, 'author2', 'password', 1, true),
                (-3, 'turista1@gmail.com', 'password', 2, true),
                (-21, 'turista2@gmail.com', 'password', 2, true)
            ON CONFLICT (""Id"") DO NOTHING;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO stakeholders.""Users"" (""Id"", ""Username"", ""Password"", ""Role"", ""IsActive"")
            SELECT DISTINCT 
                t.""AuthorId"",
                CONCAT('author', t.""AuthorId""),
                'password',
                1,
                true
            FROM tours.""Tours"" t
            WHERE NOT EXISTS (
                SELECT 1 FROM stakeholders.""Users"" u 
                WHERE u.""Id"" = t.""AuthorId""
            )
            ON CONFLICT (""Id"") DO NOTHING;
        ");

        // 2. Kreiraj People
        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO stakeholders.""People"" (""Id"", ""UserId"", ""Name"", ""Surname"", ""Email"")
            VALUES 
                (-1, -1, 'Author', 'One', 'author1@test.com'),
                (-2, -2, 'Author', 'Two', 'author2@test.com'),
                (-3, -3, 'Tourist', 'Test', 'turista1@gmail.com'),
                (-21, -21, 'Tourist', 'Test2', 'turista2@gmail.com')
            ON CONFLICT (""Id"") DO NOTHING;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO stakeholders.""People"" (""Id"", ""UserId"", ""Name"", ""Surname"", ""Email"")
            SELECT DISTINCT 
                t.""AuthorId"",
                t.""AuthorId"",
                'Author',
                CONCAT('User', t.""AuthorId""),
                CONCAT('author', t.""AuthorId"", '@test.com')
            FROM tours.""Tours"" t
            WHERE NOT EXISTS (
                SELECT 1 FROM stakeholders.""People"" p 
                WHERE p.""Id"" = t.""AuthorId""
            )
            ON CONFLICT (""Id"") DO NOTHING;
        ");
    }

    private async Task EnsurePurchaseToken(ToursContext dbContext, long touristId, long tourId)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO tours.""TourPurchaseTokens"" (""Id"", ""UserId"", ""TourId"")
              SELECT 
                  (SELECT COALESCE(MIN(""Id""), 0) - 1 FROM tours.""TourPurchaseTokens""),
                  {0}, 
                  {1}
              WHERE NOT EXISTS (
                  SELECT 1 FROM tours.""TourPurchaseTokens"" 
                  WHERE ""UserId"" = {0} AND ""TourId"" = {1}
              )",
            touristId, tourId);
    }

    private async Task EnsureTourHasDuration(ToursContext dbContext, long tourId)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO tours.""TourDurations"" (""TourId"", ""TransportType"", ""DurationInMinutes"") 
              SELECT {0}, {1}, {2}
              WHERE NOT EXISTS (
                  SELECT 1 FROM tours.""TourDurations"" 
                  WHERE ""TourId"" = {0}
              )",
            tourId, 1, 120);
    }

    private async Task CleanupPersons(ToursContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync(@"
            DELETE FROM stakeholders.""People"" 
            WHERE ""Id"" IN (-1, -2, -3, -21)
            OR ""Email"" LIKE 'author%@test.com';
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            DELETE FROM stakeholders.""Users"" 
            WHERE ""Id"" IN (-1, -2, -3, -21)
            OR ""Username"" LIKE 'author%';
        ");
    }

    private static TourPlaylistController CreateController(IServiceScope scope, string userId)
    {
        return new TourPlaylistController(
            scope.ServiceProvider.GetRequiredService<ITourPlaylistService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}