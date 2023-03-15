namespace API.Interface
{
    public interface ISoftDelete
    {
        long? DeleteUserId { get; set; }
        DateTime? DeletionTime { get; set; }
        bool IsDeleted { get; set; }
    }
}
