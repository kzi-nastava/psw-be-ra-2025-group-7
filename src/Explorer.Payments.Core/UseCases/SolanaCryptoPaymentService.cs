using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Explorer.Payments.Core.UseCases
{
    public class SolanaCryptoPaymentService : ICryptoPaymentService
    {
        private readonly ICryptoDepositRequestRepository _depositRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentNotificationRepository _notificationRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string PROJECT_WALLET_ADDRESS = "66NRnYFtAovyi4qFUbmbrD4ZdiLui2QZR8RyJnor4tXG";
        private const string SOLANA_RPC_URL = "https://api.devnet.solana.com";
        private const decimal EXCHANGE_RATE = 1000m; // 1 SOL = 1000 AC
        private const decimal MINIMUM_DEPOSIT = 0.01m; // SOL
        private const string NETWORK_NAME = "Solana Devnet";

        public SolanaCryptoPaymentService(
            ICryptoDepositRequestRepository depositRepository,
            IWalletRepository walletRepository,
            IPaymentNotificationRepository notificationRepository,
            IHttpClientFactory httpClientFactory)
        {
            _depositRepository = depositRepository;
            _walletRepository = walletRepository;
            _notificationRepository = notificationRepository;
            _httpClientFactory = httpClientFactory;
        }

        public CryptoWalletInfoDto GetWalletInfo()
        {
            var instructions = $"1. Send SOL to the wallet address shown above on {NETWORK_NAME}.\n" +
                              $"2. Minimum deposit: {MINIMUM_DEPOSIT} SOL.\n" +
                              $"3. Exchange rate: 1 SOL = {EXCHANGE_RATE} AC.\n" +
                              "4. Register your Phantom wallet address first.\n" +
                              "5. After sending, the transaction will be automatically detected within 30 seconds.\n" +
                              "6. You can also manually submit the transaction ID for faster processing.";

            return new CryptoWalletInfoDto(
                PROJECT_WALLET_ADDRESS,
                PROJECT_WALLET_ADDRESS, // QR code can be generated on frontend
                NETWORK_NAME,
                MINIMUM_DEPOSIT,
                EXCHANGE_RATE,
                instructions
            );
        }

        public void RegisterSolanaAddress(long userId, string solanaAddress)
        {
            var wallet = _walletRepository.GetByUserId(userId);
            wallet.RegisterSolanaAddress(solanaAddress);
            _walletRepository.Update(wallet);
        }

        public UserSolanaAddressDto GetUserSolanaAddress(long userId)
        {
            var wallet = _walletRepository.GetByUserId(userId);
            var isRegistered = !string.IsNullOrWhiteSpace(wallet.SolanaWalletAddress);

            return new UserSolanaAddressDto(wallet.SolanaWalletAddress, isRegistered);
        }

        public CryptoDepositRequestDto SubmitTransaction(long userId, string transactionId)
        {
            // Check if transaction already exists
            var existing = _depositRepository.GetByTransactionId(transactionId);
            if (existing != null)
            {
                return MapToDto(existing);
            }

            // Create pending deposit request with user context
            var request = new CryptoDepositRequest(userId, transactionId, 0, 0);
            _depositRepository.Create(request);

            return MapToDto(request);
        }

        public async Task ProcessPendingDeposits()
        {
            try
            {
                Console.WriteLine("Starting ProcessPendingDeposits...");

                // Step 1: Get recent transactions from blockchain
                var recentTransactions = await FetchRecentTransactionsAsync();
                Console.WriteLine($"Fetched {recentTransactions.Count} recent transactions from blockchain");

                // Step 2: Process each transaction from the blockchain
                foreach (var txId in recentTransactions)
                {
                    try
                    {
                        // Check if this transaction has already been processed
                        var existingDeposit = _depositRepository.GetByTransactionId(txId);
                        if (existingDeposit != null)
                        {
                            Console.WriteLine($"Transaction {txId} already exists with status {existingDeposit.Status}, skipping...");
                            
                            // If it's pending, try to process it
                            if (existingDeposit.Status == CryptoDepositStatus.Pending)
                            {
                                var txDetails = await GetTransactionDetailsAsync(txId);
                                if (txDetails != null)
                                {
                                    await ProcessTransactionForDeposit(existingDeposit, txDetails);
                                }
                            }
                            continue;
                        }

                        Console.WriteLine($"New transaction detected: {txId}, fetching details...");

                        // Fetch transaction details
                        var transactionDetails = await GetTransactionDetailsAsync(txId);
                        if (transactionDetails == null)
                        {
                            Console.WriteLine($"Could not get details for transaction {txId}");
                            continue;
                        }

                        Console.WriteLine($"Transaction details: Sender={transactionDetails.SenderAddress}, Amount={transactionDetails.AmountInSol} SOL");

                        // Find user wallet by sender address
                        var wallet = _walletRepository.GetByUserSolanaAddress(transactionDetails.SenderAddress);
                        if (wallet == null)
                        {
                            Console.WriteLine($"No wallet found with Solana address {transactionDetails.SenderAddress}, skipping transaction");
                            continue;
                        }

                        Console.WriteLine($"Found wallet for user {wallet.UserId}, creating deposit request...");

                        // Create deposit request for this transaction
                        var coinsAmount = transactionDetails.AmountInSol * EXCHANGE_RATE;
                        var newDeposit = new CryptoDepositRequest(
                            wallet.UserId,
                            txId,
                            transactionDetails.AmountInSol,
                            coinsAmount,
                            transactionDetails.SenderAddress
                        );
                        _depositRepository.Create(newDeposit);

                        Console.WriteLine($"Created deposit request {newDeposit.Id}, processing...");

                        // Process the newly created deposit
                        await ProcessTransactionForDeposit(newDeposit, transactionDetails);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing transaction {txId}: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }

                // Step 3: Also process any manually submitted pending deposits
                var manualPendingDeposits = _depositRepository.GetPendingDeposits();
                Console.WriteLine($"Found {manualPendingDeposits.Count} manually submitted pending deposits");

                foreach (var deposit in manualPendingDeposits)
                {
                    try
                    {
                        Console.WriteLine($"Processing manual deposit {deposit.Id} for transaction {deposit.TransactionId}");
                        var txDetails = await GetTransactionDetailsAsync(deposit.TransactionId);
                        if (txDetails != null)
                        {
                            await ProcessTransactionForDeposit(deposit, txDetails);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing manual deposit {deposit.Id}: {ex.Message}");
                    }
                }

                Console.WriteLine("ProcessPendingDeposits completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessPendingDeposits: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task<List<string>> FetchRecentTransactionsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                
                var requestBody = new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    method = "getSignaturesForAddress",
                    @params = new object[]
                    {
                        PROJECT_WALLET_ADDRESS,
                        new { limit = 20 }
                    }
                };

                var response = await httpClient.PostAsJsonAsync(SOLANA_RPC_URL, requestBody);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"getSignaturesForAddress response: {content}");

                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                var signatures = new List<string>();
                if (jsonResponse.TryGetProperty("result", out var result) && result.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in result.EnumerateArray())
                    {
                        if (item.TryGetProperty("signature", out var sig))
                        {
                            signatures.Add(sig.GetString()!);
                        }
                    }
                }

                return signatures;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recent transactions: {ex.Message}");
                return new List<string>();
            }
        }

        private async Task<TransactionDetails?> GetTransactionDetailsAsync(string transactionId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var requestBody = new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    method = "getTransaction",
                    @params = new object[]
                    {
                        transactionId,
                        new { encoding = "jsonParsed", maxSupportedTransactionVersion = 0 }
                    }
                };

                var response = await httpClient.PostAsJsonAsync(SOLANA_RPC_URL, requestBody);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"getTransaction response for {transactionId}: {content.Substring(0, Math.Min(500, content.Length))}...");

                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonResponse.TryGetProperty("result", out var result) || result.ValueKind == JsonValueKind.Null)
                {
                    Console.WriteLine($"Transaction {transactionId} result is null");
                    return null;
                }

                // Check if transaction failed
                if (result.TryGetProperty("meta", out var meta) && 
                    meta.TryGetProperty("err", out var err) && 
                    err.ValueKind != JsonValueKind.Null)
                {
                    Console.WriteLine($"Transaction {transactionId} failed on blockchain");
                    return null;
                }

                // Extract transaction details
                var txDetails = new TransactionDetails { TransactionId = transactionId };

                // Get sender and amount from instructions
                if (result.TryGetProperty("transaction", out var transaction) &&
                    transaction.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("instructions", out var instructions))
                {
                    foreach (var instruction in instructions.EnumerateArray())
                    {
                        if (instruction.TryGetProperty("parsed", out var parsed) &&
                            parsed.TryGetProperty("type", out var type) &&
                            type.GetString() == "transfer")
                        {
                            if (parsed.TryGetProperty("info", out var info))
                            {
                                if (info.TryGetProperty("source", out var source))
                                    txDetails.SenderAddress = source.GetString();

                                if (info.TryGetProperty("destination", out var dest) &&
                                    dest.GetString() == PROJECT_WALLET_ADDRESS &&
                                    info.TryGetProperty("lamports", out var lamports))
                                {
                                    txDetails.AmountInLamports = lamports.GetInt64();
                                    txDetails.AmountInSol = txDetails.AmountInLamports / 1_000_000_000m;
                                }
                            }
                        }
                    }
                }

                if (txDetails.AmountInSol <= 0)
                {
                    Console.WriteLine($"Transaction {transactionId} has no valid SOL transfer to project wallet");
                    return null;
                }

                return txDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transaction details for {transactionId}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        private async Task ProcessTransactionForDeposit(CryptoDepositRequest deposit, TransactionDetails txDetails)
        {
            try
            {
                Console.WriteLine($"Processing deposit {deposit.Id} with transaction details...");

                // Validate minimum deposit
                if (txDetails.AmountInSol < MINIMUM_DEPOSIT)
                {
                    Console.WriteLine($"Amount {txDetails.AmountInSol} is below minimum {MINIMUM_DEPOSIT}, failing deposit");
                    deposit.Fail();
                    _depositRepository.Update(deposit);
                    return;
                }

                // Calculate coins amount
                var coinsAmount = txDetails.AmountInSol * EXCHANGE_RATE;
                Console.WriteLine($"Calculated coins amount: {coinsAmount} AC");

                // Get user wallet
                Wallet? wallet = null;
                try
                {
                    wallet = _walletRepository.GetByUserId(deposit.UserId);
                    Console.WriteLine($"Found wallet for user {deposit.UserId}, current balance: {wallet.Balance}");
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine($"No wallet found for user {deposit.UserId}, trying to find by Solana address");
                    if (!string.IsNullOrWhiteSpace(txDetails.SenderAddress))
                    {
                        wallet = _walletRepository.GetByUserSolanaAddress(txDetails.SenderAddress);
                    }

                    if (wallet == null)
                    {
                        Console.WriteLine($"Could not find wallet by Solana address either, failing deposit");
                        deposit.Fail();
                        _depositRepository.Update(deposit);
                        return;
                    }
                }

                // Verify sender address matches if wallet has registered address
                if (!string.IsNullOrWhiteSpace(wallet.SolanaWalletAddress) &&
                    !string.IsNullOrWhiteSpace(txDetails.SenderAddress) &&
                    !wallet.SolanaWalletAddress.Equals(txDetails.SenderAddress, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Sender address mismatch: wallet has {wallet.SolanaWalletAddress} but transaction from {txDetails.SenderAddress}");
                    deposit.Fail();
                    _depositRepository.Update(deposit);
                    return;
                }

                // Add funds to wallet
                Console.WriteLine($"Adding {coinsAmount} AC to wallet...");
                wallet.AddFunds(coinsAmount);
                _walletRepository.Update(wallet);
                Console.WriteLine($"Wallet updated, new balance: {wallet.Balance}");

                // Confirm deposit
                var explorerUrl = $"https://explorer.solana.com/tx/{txDetails.TransactionId}?cluster=devnet";
                deposit.Confirm(explorerUrl);
                _depositRepository.Update(deposit);
                Console.WriteLine($"Deposit {deposit.Id} confirmed and updated");

                // Create notification
                var notification = new PaymentNotification(
                    wallet.UserId,
                    $"?? Crypto deposit confirmed! {txDetails.AmountInSol:F4} SOL ? {coinsAmount:F2} AC added to your wallet."
                );
                _notificationRepository.Create(notification);
                Console.WriteLine($"Notification created for user {wallet.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing transaction for deposit {deposit.Id}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                deposit.Fail();
                _depositRepository.Update(deposit);
            }
        }

        public List<CryptoDepositRequestDto> GetUserDepositHistory(long userId)
        {
            var deposits = _depositRepository.GetByUserId(userId);
            return deposits.Select(MapToDto).ToList();
        }

        private CryptoDepositRequestDto MapToDto(CryptoDepositRequest request)
        {
            return new CryptoDepositRequestDto(
                request.Id,
                request.UserId,
                request.TransactionId,
                request.CryptoAmount,
                request.CoinsAmount,
                request.Status.ToString(),
                request.RequestedAt,
                request.ConfirmedAt,
                request.BlockchainExplorerUrl,
                request.SenderWalletAddress
            );
        }

        private class TransactionDetails
        {
            public string TransactionId { get; set; } = string.Empty;
            public string? SenderAddress { get; set; }
            public long AmountInLamports { get; set; }
            public decimal AmountInSol { get; set; }
        }
    }
}
