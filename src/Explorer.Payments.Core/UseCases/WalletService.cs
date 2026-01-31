using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Payments.Core.UseCases
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentNotificationRepository _paymentNotificationRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public WalletService(
            IWalletRepository walletRepository, 
            IPaymentNotificationRepository paymentNotificationRepository,
            IRealTimeNotificationService realTimeNotificationService)
        {
            _walletRepository = walletRepository;
            _paymentNotificationRepository = paymentNotificationRepository;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public void CreateWallet(long userId)
        {
            if (_walletRepository.ExistsForUser(userId))
                return;

            var wallet = new Wallet(userId);
            _walletRepository.Create(wallet);
        }
        
        public decimal GetBalance(long userId)
        {
            return _walletRepository.GetByUserId(userId).Balance;
        }

        public async Task AddFunds(long touristUserId, decimal amount)
        {
            var wallet = _walletRepository.GetByUserId(touristUserId);
            wallet.AddFunds(amount);
            _walletRepository.Update(wallet);

            var notification = new PaymentNotification(
                touristUserId,
                $"Uplaćeno {amount} AC. Trenutno stanje: {wallet.Balance} AC."
            );

            _paymentNotificationRepository.Create(notification);

            // Send real-time SignalR notifications
            try
            {
                await _realTimeNotificationService.SendDepositConfirmationAsync(
                    touristUserId,
                    amount,
                    wallet.Balance
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SignalR notification: {ex.Message}");
                // Don't fail the operation if SignalR fails
            }
        }

        public WalletDto GetWallet(long userId)
        {
            var wallet = _walletRepository.GetByUserId(userId);
            return new WalletDto(wallet.Balance);
        }
    }
}
