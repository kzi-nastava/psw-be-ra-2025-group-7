using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourPlaylistService : ITourPlaylistService
{
    private readonly ITourPlaylistRepository _playlistRepository;
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IDeezerService _deezerService;
    private readonly IWeatherService _weatherService;
    private readonly IMapper _mapper;

    // Mapiranje žanrova prema regijama
    private static readonly Dictionary<string, List<string>> REGIONAL_GENRES = new()
    {
        { "southern_europe", new() { "mediterranean", "latin", "folk" } },
        { "western_europe", new() { "pop", "indie", "electronic" } },
        { "eastern_europe", new() { "folk", "electronic" } },
        { "northern_europe", new() { "indie", "metal" } },
        { "east_asia", new() { "pop", "indie" } },
        { "southeast_asia", new() { "pop", "indie" } },
        { "south_asia", new() { "pop", "indie" } },
        { "north_america", new() { "pop", "rock", "hip-hop", "country" } },
        { "latin_america", new() { "latin", "reggae", "pop" } },
        { "africa", new() { "reggae", "world", "pop" } },
        { "oceania", new() { "indie", "pop", "reggae", "alternative" } },
        { "middle_east", new() { "pop", "world" } }
    };

    // Mapiranje žanrova prema državama
    private static readonly Dictionary<string, string> COUNTRY_TO_REGION = new()
    {
        { "italy", "southern_europe" },
        { "spain", "southern_europe" },
        { "france", "western_europe" },
        { "germany", "western_europe" },
        { "serbia", "eastern_europe" },
        { "poland", "eastern_europe" },
        { "norway", "northern_europe" },
        { "sweden", "northern_europe" },
        { "japan", "east_asia" },
        { "china", "east_asia" },
        { "thailand", "southeast_asia" },
        { "india", "south_asia" },
        { "usa", "north_america" },
        { "canada", "north_america" },
        { "mexico", "latin_america" },
        { "brazil", "latin_america" },
        { "south africa", "africa" },
        { "egypt", "africa" },
        { "australia", "oceania" },
        { "new zealand", "oceania" }
    };

    // Mapiranje po vremenskoj prognozi 
    private static readonly Dictionary<string, (double energy, double valence)> WEATHER_MOODS = new()
    {
        { "sunny", (0.75, 0.85) },
        { "rainy", (0.35, 0.45) },
        { "cloudy", (0.55, 0.60) },
        { "snowy", (0.50, 0.70) }
    };

    // Mapiranje na osnovu doba dana
    private static readonly Dictionary<string, (double energy, int tempo)> TIME_MOODS = new()
    {
        { "morning", (0.70, 110) },
        { "afternoon", (0.75, 120) },
        { "evening", (0.60, 100) },
        { "night", (0.80, 125) }
    };

    public TourPlaylistService(
        ITourPlaylistRepository playlistRepository,
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        IDeezerService deezerService,
        IWeatherService weatherService,
        IMapper mapper)
    {
        _playlistRepository = playlistRepository;
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _deezerService = deezerService;
        _weatherService = weatherService;
        _mapper = mapper;
    }

    public async Task<TourPlaylistDto> GeneratePlaylist(long touristId, long executionId, GeneratePlaylistDto dto)
    {
        var execution = _executionRepository.Get(executionId);
        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only generate playlists for your own tour executions.");

        if (execution.Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Cannot generate playlist for inactive tour execution.");

        if (dto.Genres == null || dto.Genres.Count == 0)
            throw new ArgumentException("At least one genre must be selected.");

        if (dto.Genres.Count > 5)
            throw new ArgumentException("Maximum 5 genres allowed.");

        var existingPlaylist = _playlistRepository.GetByTourExecutionId(executionId);
        if (existingPlaylist != null)
        {
            _playlistRepository.Delete(executionId);
        }

        var tour = _tourRepository.Get(execution.TourId);

        var tourDurationMinutes = CalculateTourDuration(tour);
        if (tourDurationMinutes == 0)
            throw new InvalidOperationException("Cannot determine tour duration. Please ensure tour has duration information.");

        WeatherDataDto? weatherData = null;
        if (dto.IncludeWeather && tour.KeyPoints.Any())
        {
            var firstKeyPoint = tour.KeyPoints.First();
            weatherData = await _weatherService.GetCurrentWeather(firstKeyPoint.Latitude, firstKeyPoint.Longitude);
        }

        var regionalGenres = GetRegionalGenres(tour);

        var combinedGenres = dto.Genres
            .Concat(regionalGenres)
            .Distinct()
            .Take(5)
            .ToList();

        var (energy, valence, tempo) = CalculateMoodParameters(weatherData);

        var numberOfTracks = (int)Math.Ceiling(tourDurationMinutes / 3.5);
        numberOfTracks = Math.Min(numberOfTracks, 50);

        var trackDtos = await _deezerService.GetRecommendations(
            combinedGenres,
            energy,
            valence,
            tempo,
            numberOfTracks
        );

        if (trackDtos.Count == 0)
            throw new InvalidOperationException("Failed to generate playlist. Please try different genres.");

        var tracks = trackDtos.Select(trackDto => new PlaylistTrack(
            trackDto.SpotifyTrackId,
            trackDto.Name,
            trackDto.Artist,
            trackDto.SpotifyUri,
            trackDto.DurationMs
        )).ToList();

        var playlist = new TourPlaylist(
            executionId,
            dto.Genres,
            dto.IncludeWeather,
            weatherData?.Condition,
            weatherData?.Temperature,
            tracks
        );

        var createdPlaylist = _playlistRepository.Create(playlist);

        return _mapper.Map<TourPlaylistDto>(createdPlaylist);
    }

    public TourPlaylistDto GetPlaylist(long touristId, long executionId)
    {
        var execution = _executionRepository.Get(executionId);
        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only access playlists for your own tour executions.");

        var playlist = _playlistRepository.GetByTourExecutionId(executionId);
        if (playlist == null)
            throw new NotFoundException("Playlist not found for this tour execution.");

        return _mapper.Map<TourPlaylistDto>(playlist);
    }

    public void DeletePlaylist(long touristId, long executionId)
    {
        var execution = _executionRepository.Get(executionId);
        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only delete playlists for your own tour executions.");

        var playlist = _playlistRepository.GetByTourExecutionId(executionId);
        if (playlist == null)
            throw new NotFoundException("Playlist not found for this tour execution.");

        _playlistRepository.Delete(executionId);
    }

    private int CalculateTourDuration(Tour tour)
    {
        if (tour.TourDurations == null || tour.TourDurations.Count == 0)
        {
            return (int)Math.Ceiling(tour.LengthInKm / 4.0 * 60);
        }

        return tour.TourDurations.Sum(d => d.Minutes);
    }

    private List<string> GetRegionalGenres(Tour tour)
    {
        var tourText = $"{tour.Name} {tour.Description}".ToLower();

        foreach (var (country, region) in COUNTRY_TO_REGION)
        {
            if (tourText.Contains(country))
            {
                return REGIONAL_GENRES.GetValueOrDefault(region, new List<string>());
            }
        }

        return new List<string> { "pop", "indie" };
    }

    private (double energy, double valence, int tempo) CalculateMoodParameters(WeatherDataDto? weatherData)
    {
        double energy = 0.65;
        double valence = 0.65;
        int tempo = 115;

        int moodCount = 1;

        if (weatherData != null && WEATHER_MOODS.TryGetValue(weatherData.Condition, out var weatherMood))
        {
            energy += weatherMood.energy;
            valence += weatherMood.valence;
            moodCount++;
        }

        var timeOfDay = GetTimeOfDay();
        if (TIME_MOODS.TryGetValue(timeOfDay, out var timeMood))
        {
            energy += timeMood.energy;
            tempo = timeMood.tempo;
            moodCount++;
        }

        energy /= moodCount;
        valence /= moodCount;

        energy = Math.Clamp(energy, 0.0, 1.0);
        valence = Math.Clamp(valence, 0.0, 1.0);

        return (energy, valence, tempo);
    }

    private string GetTimeOfDay()
    {
        var hour = DateTime.UtcNow.Hour;
        return hour switch
        {
            >= 6 and < 12 => "morning",
            >= 12 and < 18 => "afternoon",
            >= 18 and < 22 => "evening",
            _ => "night"
        };
    }
}