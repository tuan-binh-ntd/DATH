namespace Entities.Interface
{
    public interface IHasLastModifierUserId<TPrimaryKey> : IHasLastModificationTime
    {
        TPrimaryKey? LastModifierUserId {get; set;}
    }
}
