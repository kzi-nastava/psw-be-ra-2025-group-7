namespace Explorer.Notifications.API.Public
{
    public interface INotificationService
    {
        void CreateProblemMessageNotification(
            long recipientUserId,   // kome šaljemo
            int problemId,         // da bi FE znao koji problem da otvori
            string messagePreview, // prvih 20–30 karaktera poruke
            DateTime createdAt);   // vreme kreiranja poruke
    }
}
