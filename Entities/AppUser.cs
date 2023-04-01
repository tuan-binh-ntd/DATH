using API.Interface;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("AppUser")]
    public class AppUser : IdentityUser<long>, IHasCreationTime, IHasCreatorUserId, IHasLastModifierUserId, IHasModificationTime, ISoftDelete
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
