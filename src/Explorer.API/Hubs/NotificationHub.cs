using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Hubs
{
    public class NotificationHub : Hub
    {
        // Ova metoda se može koristiti ako frontend želi da eksplicitno pošalje poruku svim klijentima,
        // ali češće će backend (servisi) slati notifikacije preko IHubContext-a.
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Primer metode za slanje notifikacije određenom korisniku (koristeći UserIdentifier mapiran iz tokena)
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // Ovde možete dodati logiku za praćenje online korisnika
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            // Ovde možete dodati logiku za čišćenje resursa
        }
    }
}
