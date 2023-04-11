using Entities.Enum.User;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dto
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
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
        public string? AvatarUrl { get; set; }
        public int? ShopId { get; set; }
    }
}
