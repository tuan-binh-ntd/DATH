using Entities.Enum.User;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Employee : BaseEntity<long>
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        public Gender Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public EmployeeType Type { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        [StringLength(12)]
        public string? IdNumber { get; set; }
        [StringLength(11)]
        public string? Phone { get; set; }

        //Relationship
        public long UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int ShopId { get; set; }
        public Shop? Shop { get; set; }
    }
}
