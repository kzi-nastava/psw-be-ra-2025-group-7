namespace Explorer.Payments.API.Dtos
{
    public class RegisterSolanaAddressDto
    {
        public string SolanaAddress { get; set; }

        public RegisterSolanaAddressDto(string solanaAddress)
        {
            SolanaAddress = solanaAddress;
        }
    }
}
