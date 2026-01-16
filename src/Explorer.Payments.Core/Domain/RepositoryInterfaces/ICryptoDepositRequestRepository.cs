using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICryptoDepositRequestRepository
    {
        CryptoDepositRequest Create(CryptoDepositRequest request);
        CryptoDepositRequest? GetById(long id);
        CryptoDepositRequest? GetByTransactionId(string transactionId);
        List<CryptoDepositRequest> GetPendingDeposits();
        List<CryptoDepositRequest> GetByUserId(long userId);
        void Update(CryptoDepositRequest request);
    }
}
