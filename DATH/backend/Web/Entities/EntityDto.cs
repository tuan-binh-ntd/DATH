using Entities.Interface;

namespace Entities
{
    public abstract class EntityDto<T> : IEntity<T?>
    {
        public T? Id { get; set; }
    }

    public abstract class EntityDto : IEntity<int?>
    {
        public int? Id { get; set; }
    }
}
