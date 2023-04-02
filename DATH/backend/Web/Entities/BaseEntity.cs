using Entities.Interface;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public abstract class BaseEntity<T> : IEntity<T>, IHasCreatorUserId<T?>, IHasLastModifierUserId<T?>, ISoftDelete<T>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual T Id { get; set; }
        public virtual T? CreatorUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime CreationTime { get; set; }
        public virtual T? LastModifierUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual T? DeleteUserId { get; set; }
        [DataType(DataType.DateTime)]
        public virtual DateTime? DeletionTime { get; set; }
        public virtual bool IsDeleted { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }
        public new int? CreatorUserId { get; set; }
        [DataType(DataType.DateTime)]
        public new DateTime CreationTime { get; set; }
        public new int? LastModifierUserId { get; set; }
        [DataType(DataType.DateTime)]
        public new DateTime? LastModificationTime { get; set; }
        public new int? DeleteUserId { get; set; }
        [DataType(DataType.DateTime)]
        public new DateTime? DeletionTime { get; set; }
        public new bool IsDeleted { get; set; }
    }
}
