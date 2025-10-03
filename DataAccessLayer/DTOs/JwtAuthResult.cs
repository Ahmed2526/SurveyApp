namespace DataAccessLayer.DTOs
{
    public class JwtAuthResult
    {
        public string Token { get; set; }
        public DateTime ExpiryInDays { get; set; }
    }
}
