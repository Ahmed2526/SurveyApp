using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class RoleRequest
    {
        [Required]
        [Length(3, 256)]
        public string Name { get; set; }

        public List<string> Permissions { get; set; }
    }
}
