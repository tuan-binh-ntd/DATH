namespace Entities.Interface
{
    public interface IHasCreatorUserId<TPrimaryKey> : IHasCreationTime
    {
        TPrimaryKey? CreatorUserId { get; set; }
    }
}
