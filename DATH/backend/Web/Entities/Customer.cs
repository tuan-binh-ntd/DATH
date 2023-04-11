using Entities.Enum.User;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Customer : BaseEntity<long>
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        [StringLength(12)]
        public string? IdNumber { get; set; }
        [StringLength(11)]
        public string? Phone { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        //Relationship
        public long UserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
