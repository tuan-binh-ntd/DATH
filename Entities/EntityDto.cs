using API.Interface;

namespace API.Entities
{
    public class EntityDto<T> : IEntity<T>
    {
        public T? Id { get; set; }
    }
}
