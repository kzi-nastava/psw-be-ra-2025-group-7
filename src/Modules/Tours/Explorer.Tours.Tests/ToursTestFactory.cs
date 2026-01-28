using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    static ToursTestFactory()
    {
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
    }

    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<EncountersContext>)
        );
        services.Remove(descriptor!);
        services.AddDbContext<EncountersContext>(SetupTestContext());

        return services;
    }
}