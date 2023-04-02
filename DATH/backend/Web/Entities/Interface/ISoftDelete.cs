namespace Entities.Interface
{
    public interface ISoftDelete<TPrimaryKey>
    {
        TPrimaryKey? DeleteUserId { get; set; }
        DateTime? DeletionTime { get; set; }
        bool IsDeleted { get; set; }
    }
}
