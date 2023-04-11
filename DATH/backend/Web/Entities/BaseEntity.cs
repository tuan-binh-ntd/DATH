using Entities.Interface;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public abstract class BaseEntity<T> : IEntity<T>, IHasCreatorUserId, IHasLastModifierUserId, ISoftDelete
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual T Id { get; set; }
        public virtual long? CreatorUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? CreationTime { get; set; }
        public virtual long? LastModifierUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual long? DeleteUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? DeletionTime { get; set; }
        public virtual bool IsDeleted { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}
