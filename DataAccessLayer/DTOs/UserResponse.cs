namespace DataAccessLayer.DTOs
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsLockedOut { get; set; }

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
