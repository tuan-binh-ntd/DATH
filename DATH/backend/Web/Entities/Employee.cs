using Entities.Enum.User;

namespace Entities
{
    public class Employee : BaseEntity<long>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Code { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthday { get; set; }
        public EmployeeType Type { get; set; }
        public string? Email { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }

        //Relationship
        public long UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int ShopId { get; set; }
        public Shop? Shop { get; set; }
    }
}
