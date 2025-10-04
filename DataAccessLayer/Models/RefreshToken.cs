using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }


        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; } = default!;
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
