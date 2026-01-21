using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class CryptoDepositRequest : Entity
    {
        public long UserId { get; private set; }
        public string TransactionId { get; private set; }
        public decimal CryptoAmount { get; private set; }
        public decimal CoinsAmount { get; private set; }
        public CryptoDepositStatus Status { get; private set; }
        public DateTime RequestedAt { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public string? BlockchainExplorerUrl { get; private set; }
        public string? SenderWalletAddress { get; private set; }

        private CryptoDepositRequest() { }

        public CryptoDepositRequest(long userId, string transactionId, decimal cryptoAmount, decimal coinsAmount, string? senderWalletAddress = null)
        {
            if (userId == 0)
                throw new ArgumentException("User ID cannot be zero.", nameof(userId));

            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            // Allow 0 amounts for manual submissions (will be populated from blockchain)
            if (cryptoAmount < 0)
                throw new ArgumentException("Crypto amount cannot be negative.", nameof(cryptoAmount));

            if (coinsAmount < 0)
                throw new ArgumentException("Coins amount cannot be negative.", nameof(coinsAmount));

            UserId = userId;
            TransactionId = transactionId;
            CryptoAmount = cryptoAmount;
            CoinsAmount = coinsAmount;
            SenderWalletAddress = senderWalletAddress;
            Status = CryptoDepositStatus.Pending;
            RequestedAt = DateTime.UtcNow;
        }

        public void Confirm(string blockchainExplorerUrl)
        {
            if (Status != CryptoDepositStatus.Pending)
                throw new InvalidOperationException("Only pending deposits can be confirmed.");

            Status = CryptoDepositStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;
            BlockchainExplorerUrl = blockchainExplorerUrl;
        }

        public void Fail()
        {
            if (Status != CryptoDepositStatus.Pending)
                throw new InvalidOperationException("Only pending deposits can be marked as failed.");

            Status = CryptoDepositStatus.Failed;
            ConfirmedAt = DateTime.UtcNow;
        }

        public void UpdateSenderAddress(string senderAddress)
        {
            if (string.IsNullOrWhiteSpace(senderAddress))
                throw new ArgumentException("Sender address cannot be empty.", nameof(senderAddress));

            SenderWalletAddress = senderAddress;
        }
    }
}
