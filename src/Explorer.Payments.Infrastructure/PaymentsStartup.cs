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
            services.AddScoped<IPaymentNotificationService, PaymentNotificationService>();

            // NEW
            services.AddScoped<IBundleService, BundleService>();
            services.AddScoped<IBundlePurchaseService, BundlePurchaseService>();
            services.AddScoped<IPurchaseNotificationService, PurchaseNotificationService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ISaleInternalService, SaleInternalService>();
            services.AddScoped<ISaleService, SaleService>();
            // NEW (crypto payments)
            services.AddScoped<ICryptoPaymentService, SolanaCryptoPaymentService>();
            services.AddScoped<ICouponInternalService, CouponInternalService>();

        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IPaymentNotificationRepository, PaymentNotificationRepository>();

            // NEW
            services.AddScoped<IBundleRepository, BundleDbRepository>();
            services.AddScoped<IBundlePurchaseRepository,BundlePurchaseDbRepository>();

            // NEW (purchase notifications)
            services.AddScoped<IPurchaseNotificationRepository, PurchaseNotificationRepository>();

            // NEW (coupons)
            services.AddScoped<ICouponRepository, CouponDbRepository>();
            services.AddScoped<IPaymentRecordRepository, PaymentRecordDbRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            // NEW (crypto deposits)
            services.AddScoped<ICryptoDepositRequestRepository, CryptoDepositRequestDbRepository>();

            // Add HttpClient for Solana RPC calls
            services.AddHttpClient();

            // NEW (crypto deposits)
            services.AddScoped<ICryptoDepositRequestRepository, CryptoDepositRequestDbRepository>();

            // Add HttpClient for Solana RPC calls
            services.AddHttpClient();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContext<PaymentsContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
        }
    }
}
