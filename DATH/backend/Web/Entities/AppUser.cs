using Entities.Enum.User;
using Entities.Interface;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class AppUser : IdentityUser<long>, IHasCreatorUserId<long?>, IHasLastModifierUserId<long?>, ISoftDelete<long?>
    {
        public UserType Type { get; set; }
        public virtual long? CreatorUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime CreationTime { get; set; }
        public virtual long? LastModifierUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual long? DeleteUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? DeletionTime { get; set; }
        public virtual bool IsDeleted { get; set; }
        //Relationship
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }
    }
}
