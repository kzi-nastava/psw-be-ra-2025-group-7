using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests;

public class EncountersTestFactory : BaseTestFactory<EncountersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<EncountersContext>));

        services.Remove(descriptor!);
        services.AddDbContext<EncountersContext>(SetupTestContext());

        var locationDescriptor = services.SingleOrDefault(d =>
        d.ServiceType == typeof(IUserProfileLocationService));

        if (locationDescriptor != null)
            services.Remove(locationDescriptor);

        services.AddScoped<IUserProfileLocationService, FakeUserProfileLocationService>();

        return services;
    }
}
