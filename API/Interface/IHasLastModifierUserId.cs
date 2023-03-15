namespace API.Interface
{
    public interface IHasLastModifierUserId : IHasModificationTime
    {
        long? LastModifierUserId { get; set; }
    }
}
