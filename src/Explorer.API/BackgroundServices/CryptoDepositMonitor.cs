using Explorer.Payments.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Explorer.API.BackgroundServices
{
    public class CryptoDepositMonitor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CryptoDepositMonitor> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public CryptoDepositMonitor(IServiceProvider serviceProvider, ILogger<CryptoDepositMonitor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CryptoDepositMonitor background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessDepositsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing crypto deposits.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("CryptoDepositMonitor background service stopped.");
        }

        private async Task ProcessDepositsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var cryptoPaymentService = scope.ServiceProvider.GetRequiredService<ICryptoPaymentService>();

            try
            {
                await cryptoPaymentService.ProcessPendingDeposits();
                _logger.LogInformation("Successfully processed pending crypto deposits.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process pending crypto deposits.");
            }
        }
    }
}
