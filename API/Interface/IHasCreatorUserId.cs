namespace API.Interface
{
    public interface IHasCreatorUserId : IHasCreationTime
    {
        long CreatorUserId { get; set; }
    }
}
