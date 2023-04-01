namespace API.Interface
{
    public interface IEntity<T>
    {
        public T Id { get; set; }
    }

    public interface IEntity : IEntity<int> 
    {
    }
}
