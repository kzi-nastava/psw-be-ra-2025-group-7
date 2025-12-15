using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        // Replace StakeholdersContext
        var stakeholdersDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(stakeholdersDescriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        // Replace ToursContext (needed for LocationService which queries Monuments)
        var toursDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        if (toursDescriptor != null)
        {
            services.Remove(toursDescriptor);
        }
        services.AddDbContext<ToursContext>(SetupTestContext());

        return services;
    }
}