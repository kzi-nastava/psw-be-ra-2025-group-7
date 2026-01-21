using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface ICryptoPaymentService
    {
        CryptoWalletInfoDto GetWalletInfo();
        void RegisterSolanaAddress(long userId, string solanaAddress);
        UserSolanaAddressDto GetUserSolanaAddress(long userId);
        CryptoDepositRequestDto SubmitTransaction(long userId, string transactionId);
        Task ProcessPendingDeposits();
        List<CryptoDepositRequestDto> GetUserDepositHistory(long userId);
    }
}
