using Entities.Interface;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class AppUser : IdentityUser<long>, IHasCreatorUserId<long?>, IHasLastModifierUserId<long?>, ISoftDelete<long?>
    {
        public AppUser(string userName) : base(userName)
        {
        }

        public long? CreatorUserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? LastModificationTime { get; set; }
        public long? DeleteUserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
