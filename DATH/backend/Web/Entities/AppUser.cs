using Entities.Enum.User;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class AppUser : IdentityUser<long>
    {
        public UserType Type { get; set; }
        //Relationship
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }
    }
}
