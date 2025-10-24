namespace DataAccessLayer.DTOs
{
    public class RoleResponse
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
