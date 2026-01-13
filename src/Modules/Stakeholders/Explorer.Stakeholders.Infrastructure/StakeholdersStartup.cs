using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Public.Tourist;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Core.UseCases.Tourist;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.UseCases.Internal;

namespace Explorer.Stakeholders.Infrastructure;

public static class StakeholdersStartup
{
    public static IServiceCollection ConfigureStakeholdersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StakeholderProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IClubService, ClubService>();
        services.AddScoped<ITourPreferencesService, TourPreferencesService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IFollowerService, FollowerService>();
        services.AddScoped<IFollowerMessageService, FollowerMessageService>();
        services.AddScoped<IClubMessageService, ClubMessageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserProfileLocationService, UserProfileLocationService>();
        
        // Register NotificationService with both repositories
        services.AddScoped<INotificationService>(provider =>
        {
            var stakeholdersRepo = provider.GetRequiredService<INotificationRepository>();
            var toursRepo = provider.GetRequiredService<Explorer.Tours.Core.Domain.RepositoryInterfaces.INotificationRepository>();
            var mapper = provider.GetRequiredService<AutoMapper.IMapper>();
            return new NotificationService(stakeholdersRepo, toursRepo, mapper);
        });
        //services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserInternalService, UserInternalService>();



    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IMessageRepository, MessageDbRepository>();
        services.AddScoped<ITourPreferencesRepository, TourPreferencesDbRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileDbRepository>();
        services.AddScoped<IClubRepository, ClubDbRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<INotificationRepository, NotificationDbRepository>();

        
        // New follower system repositories
        services.AddScoped<IFollowerRepository, FollowerDbRepository>();
        services.AddScoped<IFollowerMessageRepository, FollowerMessageDbRepository>();
        services.AddScoped<IClubMessageRepository, ClubMessageDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
