namespace DataAccessLayer.DTOs
{
    public class RefreshTokenRequest
    {
        public string refreshToken { get; set; }
        public string oldToken { get; set; }
    }
}
