namespace API.Interface
{
    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; set; }
    }
}
