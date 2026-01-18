namespace Explorer.Payments.API.Dtos
{
    public class SubmitCryptoTransactionDto
    {
        public string TransactionId { get; set; }

        public SubmitCryptoTransactionDto(string transactionId)
        {
            TransactionId = transactionId;
        }
    }
}
