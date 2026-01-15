namespace Explorer.Payments.API.Dtos
{
    public class CryptoDepositRequestDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TransactionId { get; set; }
        public decimal CryptoAmount { get; set; }
        public decimal CoinsAmount { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? BlockchainExplorerUrl { get; set; }
        public string? SenderWalletAddress { get; set; }

        public CryptoDepositRequestDto(long id, long userId, string transactionId, decimal cryptoAmount, decimal coinsAmount, string status, DateTime requestedAt, DateTime? confirmedAt, string? blockchainExplorerUrl, string? senderWalletAddress)
        {
            Id = id;
            UserId = userId;
            TransactionId = transactionId;
            CryptoAmount = cryptoAmount;
            CoinsAmount = coinsAmount;
            Status = status;
            RequestedAt = requestedAt;
            ConfirmedAt = confirmedAt;
            BlockchainExplorerUrl = blockchainExplorerUrl;
            SenderWalletAddress = senderWalletAddress;
        }
    }
}
