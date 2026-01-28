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
public class TourPlaylistIntegrationTests : BaseToursIntegrationTest
{
    public TourPlaylistIntegrationTests(ToursTestFactory factory) : base(factory) { }

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task GeneratePlaylist_Creates_Playlist_Successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task GeneratePlaylist_With_Weather_Includes_Weather_Data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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
    public async Task GeneratePlaylist_Fails_When_Execution_Not_Active()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });
        executionService.CompleteTour(-3, execution.Id);

        var dto = new GeneratePlaylistDto
        {
            Genres = new List<string> { "pop" },
            IncludeWeather = false
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await controller.GeneratePlaylist(execution.Id, dto));
    }

    [Fact]
    public async Task GeneratePlaylist_Fails_With_No_Genres()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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

        await EnsureTourHasDuration(dbContext, -3);

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

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task GeneratePlaylist_Replaces_Existing_Playlist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task GetPlaylist_Returns_Existing_Playlist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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

        await EnsureTourHasDuration(dbContext, -3);

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

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task GetPlaylist_Throws_When_Not_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task DeletePlaylist_Removes_Playlist_Successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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
    public async Task DeletePlaylist_Throws_When_Not_Found()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-3");
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

        var execution = executionService.StartTour(-3, new StartTourExecutionDto
        {
            TourId = -3,
            Latitude = 45.2551,
            Longitude = 19.8636
        });

        // Act & Assert - No playlist to delete
        Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.NotFoundException>(() =>
            controller.DeletePlaylist(execution.Id));
    }

    [Fact(Skip = "Requires Spotify API credentials - waiting for registration to open")]
    public async Task DeletePlaylist_Throws_When_Not_Owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var executionService = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        await EnsureTourHasDuration(dbContext, -3);

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

    /// <summary>
    /// Ensures a tour has at least one duration by adding one via raw SQL if needed.
    /// Uses INSERT with WHERE NOT EXISTS to avoid duplicates.
    /// </summary>
    /// <summary>
    private async Task EnsureTourHasDuration(ToursContext dbContext, long tourId)
    {
        // Single SQL query that inserts only if duration doesn't exist
        // TransportType.Walking = 1, Duration = 120 minutes
        await dbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO tours.""TourDurations"" (""TourId"", ""TransportType"", ""DurationInMinutes"") 
          SELECT {0}, {1}, {2}
          WHERE NOT EXISTS (
              SELECT 1 FROM tours.""TourDurations"" 
              WHERE ""TourId"" = {0}
          )",
            tourId, 1, 120);
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