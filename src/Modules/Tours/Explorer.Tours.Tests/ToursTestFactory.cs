using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.API.Internal;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Tests.Integration.Tourist;
namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
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
