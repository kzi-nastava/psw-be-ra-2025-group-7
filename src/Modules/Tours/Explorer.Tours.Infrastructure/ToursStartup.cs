using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Notifications.API.Public;
using Explorer.Notifications.Infrastructure;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Explorer.Tours.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.UseCases.Author;
using Explorer.Tours.Core.UseCases.ExternalServices;
using Microsoft.Extensions.Http;


namespace Explorer.Tours.Infrastructure;

public static class ToursStartup
{
    public static IServiceCollection ConfigureToursModule(this IServiceCollection services)
    {
        // Registers all profiles since it works on the assembly
        services.AddAutoMapper(typeof(ToursProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<ITourProblemService, TourProblemService>();
        services.AddScoped<IPublicPointRequestRepository, PublicPointRequestRepository>();
        services.AddScoped<IMonumentService, MonumentService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<PublicPointRequestService>();
        services.AddScoped<ITouristEquipmentService, TouristEquipmentService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<PublicPointRequestAdminService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<ITourJournalService, TourJournalService>();
        services.AddScoped<ITouristEquipmentService, TouristEquipmentService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<ITourPurchaseTokenService, TourPurchaseTokenService>();
        services.AddScoped<ITourBrowsingService, TourBrowsingService>();
        services.AddScoped<IAnnualAwardService, AnnualAwardService>();
        services.AddScoped<ITouristToursService, TouristToursService>();
        services.AddScoped<ITourSearchService, TourSearchService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ITourExecutionService, TourExecutionService>();
        services.AddScoped<ITourReviewService, TourReviewService>();
        services.AddScoped<IEnhancedReviewService, EnhancedReviewService>();
        services.AddScoped<ITourRequestService, TourRequestService>();
        services.AddScoped<IAuthorTourRequestService, AuthorTourRequestService>();
        services.AddScoped<ITourPlaylistService, TourPlaylistService>();
        services.AddScoped<IDeezerService, DeezerService>();
        services.AddHttpClient<IDeezerService, DeezerService>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddHttpClient<IWeatherService, WeatherService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IEquipmentRepository, EquipmentDbRepository>();
        services.AddScoped<ITourProblemRepository, TourProblemDbRepository>();
        services.AddScoped<IAnnualAwardRepository, AnnualAwardRepository>();
        services.AddScoped<IMonumentRepository, MonumentDbRepository>();
        services.AddScoped<IFacilityRepository, FacilityDbRepository>();
        services.AddScoped<ITourRepository, TourDbRepository>();
        services.AddScoped<ITourJournalRepository, TourJournalDbRepository>();
        services.AddScoped<IEnhancedReviewRepository, EnhancedReviewDbRepository>();
        services.AddScoped<ITouristEquipmentRepository, TouristEquipmentRepository>();
        services.AddScoped<IFacilityRepository, FacilityDbRepository>();
        services.AddScoped<ITourRepository, TourDbRepository>();
        services.AddScoped<ITouristEquipmentRepository, TouristEquipmentRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();
        services.AddScoped<ITourExecutionRepository, TourExecutionDbRepository>();
        services.AddScoped<ITourReviewRepository, TourReviewDbRepository>();
        services.AddScoped<ITourRequestRepository, TourRequestDbRepository>();
        services.AddScoped<ITourPlaylistRepository, TourPlaylistDbRepository>();


        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));
    }
}
