namespace Explorer.BuildingBlocks.Core.UseCases
{
    public interface IRealTimeNotificationService
    {
        Task SendToUserAsync(long userId, string message);
        Task SendToAllAsync(string message);
        Task SendWalletUpdateAsync(long userId, decimal newBalance);
        Task SendDepositConfirmationAsync(long userId, decimal amount, decimal newBalance);
    }
}
