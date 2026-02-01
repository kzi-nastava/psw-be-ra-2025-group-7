using Explorer.API.Hubs;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Services
{
    public class RealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealTimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToAllAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", message);
        }

        public async Task SendToUserAsync(long userId, string message)
        {
            // Pretpostavka je da se userId koristi kao identifikator korisnika u SignalR-u.
            // Ovo zavisi od toga kako je konfigurisan IUserIdProvider u SignalR-u.
            // Po defaultu je to User.Identity.Name (što može biti username ili ID, zavisno od tokena).
            // Ako je userId long, verovatno ga treba konvertovati u string.
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task SendWalletUpdateAsync(long userId, decimal newBalance)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveWalletUpdate", new
            {
                balance = newBalance,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task SendDepositConfirmationAsync(long userId, decimal amount, decimal newBalance)
        {
            // Send wallet update
            await SendWalletUpdateAsync(userId, newBalance);

            // Also send a general notification about the deposit
            var message = $"Crypto deposit confirmed! {amount:F2} AC added to your wallet. New balance: {newBalance:F2} AC";
            await SendToUserAsync(userId, message);
        }
    }
}
