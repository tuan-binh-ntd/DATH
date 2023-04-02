using Entities.Interface;

namespace Entities
{
    public class EntityDto<T> : IEntity<T?>
    {
        public T? Id { get; set; }
    }

    public class EntityDto : IEntity<int?>
    {
        public int? Id { get; set; }
    }
}
