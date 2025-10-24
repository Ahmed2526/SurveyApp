using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Models
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }

    }
}
