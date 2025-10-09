namespace DataAccessLayer.DTOs
{
    public class AuthResult
    {
        public string JwtToken { get; set; }
        public DateTime ExpiryAt { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryAt { get; set; }

    }
}
