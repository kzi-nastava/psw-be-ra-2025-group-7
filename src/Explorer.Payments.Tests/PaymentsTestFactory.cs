using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Tests
{
    public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            // Replace PaymentsContext
            var paymentsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
            services.Remove(paymentsDescriptor!);
            services.AddDbContext<PaymentsContext>(SetupTestContext());

            // Replace StakeholdersContext
            var stakeholdersDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
            services.Remove(stakeholdersDescriptor!);
            services.AddDbContext<StakeholdersContext>(SetupTestContext());

            // Replace ToursContext (needed for tour validation in shopping cart)
            var toursDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
            if (toursDescriptor != null)
            {
                services.Remove(toursDescriptor);
            }
            services.AddDbContext<ToursContext>(SetupTestContext());

            return services;
        }
    }
}
