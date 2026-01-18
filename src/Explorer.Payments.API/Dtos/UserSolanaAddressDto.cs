namespace Explorer.Payments.API.Dtos
{
    public class UserSolanaAddressDto
    {
        public string? SolanaAddress { get; set; }
        public bool IsRegistered { get; set; }

        public UserSolanaAddressDto(string? solanaAddress, bool isRegistered)
        {
            SolanaAddress = solanaAddress;
            IsRegistered = isRegistered;
        }
    }
}
