namespace DataAccessLayer.DTOs
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int JwtExpiryInMinutes { get; set; }
        public int refreshTokenExpiryInDays { get; set; }
    }
}
