namespace TestApiFaisabilite_KronoGeo.Infrastructure.ModelsDTO
{
    public class LoginRequest
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Token { get; set; } = null;
    }
}
