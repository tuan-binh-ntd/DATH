namespace Entities.Interface
{
    public interface IHasLastModifierUserId : IHasLastModificationTime
    {
        long? LastModifierUserId {get; set;}
    }
}
