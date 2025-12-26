using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain;
using System;

namespace Explorer.Payments.Core.UseCases
{
    public class PaymentInternalService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;

        public PaymentInternalService(IWalletRepository walletRepository, IPaymentRecordRepository paymentRecordRepository)
        {
            _walletRepository = walletRepository;
            _paymentRecordRepository = paymentRecordRepository;
        }

        public bool ChargeWallet(long userId, long? tourId, long? bundleId, decimal amount, string description)
        {
            if (amount <= 0) return false;

            var wallet = _walletRepository.GetByUserId(userId);
            if (wallet == null) return false;

            if (wallet.Balance < amount) return false;

            // Deduct funds and persist
            wallet.Deduct(amount);
            _walletRepository.Update(wallet);

            // Create payment record
            var record = new PaymentRecord(userId, tourId, bundleId, amount, DateTime.UtcNow, description);
            _paymentRecordRepository.Create(record);

            return true;
        }
    }
}
