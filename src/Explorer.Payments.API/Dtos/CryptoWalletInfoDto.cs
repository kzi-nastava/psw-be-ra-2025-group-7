namespace Explorer.Payments.API.Dtos
{
    public class CryptoWalletInfoDto
    {
        public string WalletAddress { get; set; }
        public string QrCodeData { get; set; }
        public string NetworkName { get; set; }
        public decimal MinimumDeposit { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Instructions { get; set; }

        public CryptoWalletInfoDto(string walletAddress, string qrCodeData, string networkName, decimal minimumDeposit, decimal exchangeRate, string instructions)
        {
            WalletAddress = walletAddress;
            QrCodeData = qrCodeData;
            NetworkName = networkName;
            MinimumDeposit = minimumDeposit;
            ExchangeRate = exchangeRate;
            Instructions = instructions;
        }
    }
}
