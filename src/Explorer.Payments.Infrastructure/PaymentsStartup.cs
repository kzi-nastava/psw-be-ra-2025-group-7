using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Payments.Infrastructure
{
    public static class PaymentsStartup
    {
        public static IServiceCollection ConfigurePaymentsModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(PaymentsProfile).Assembly);
            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IWalletInternalService, WalletInternalService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<Explorer.Payments.Core.UseCases.PaymentInternalService>();
            services.AddScoped<IPaymentInternalService>(sp => sp.GetRequiredService<Explorer.Payments.Core.UseCases.PaymentInternalService>());
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IPaymentRecordRepository, PaymentRecordRepository>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContext<PaymentsContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
        }
    }
}
