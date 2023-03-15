using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using API.Interface;

namespace API.Entities
{
    public abstract class BaseEntity<T> : IEntity<T>, IHasCreationTime, IHasCreatorUserId, IHasLastModifierUserId, IHasModificationTime, ISoftDelete
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }
        public long CreatorUserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? LastModificationTime { get; set; }
        public long? DeleteUserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
