using Entities.Enum.User;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class AppUser : IdentityUser<long>
    {
        public UserType Type { get; set; }
        [StringLength(1000)]
        public string? AvatarUrl { get; set; }
        //Relationship
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }
    }
}
