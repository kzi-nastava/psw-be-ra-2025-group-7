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
    }
}
