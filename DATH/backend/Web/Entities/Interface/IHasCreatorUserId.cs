namespace Entities.Interface
{
    public interface IHasCreatorUserId : IHasCreationTime
    {
        long? CreatorUserId { get; set; }
    }
}
