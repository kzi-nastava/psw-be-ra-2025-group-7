using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.API.Internal;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Core.Mappers;
using Explorer.Encounters.Core.UseCases;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Encounters.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;

namespace Explorer.Encounters.Infrastructure;

public static class EncountersStartup
{
    public static IServiceCollection ConfigureEncountersModule(this IServiceCollection services)
    {
        
        services.AddAutoMapper(typeof(EncountersProfile).Assembly);   // Preduslov da imamo ovu liniju koda je da smo definisali već Profile klasu u Core/Mappers
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
       
        services.AddScoped<IEncounterService, EncounterService>();
        services.AddScoped<IEncounterProgressService, EncounterProgressService>();
        services.AddScoped<IUserLocationChangedNotifier, UserLocationChangedNotifier>();
      
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("encounters"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<EncountersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "encounters")));

        services.AddScoped<IEncounterRepository, EncounterRepository>();
        services.AddScoped<IEncounterProgressRepository, EncounterProgressRepository>();
        services.AddScoped<IUserLocationChangedNotifier, UserLocationChangedNotifier>();
        


    }
}
