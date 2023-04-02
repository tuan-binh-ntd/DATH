using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class AppRole : IdentityRole<long>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
