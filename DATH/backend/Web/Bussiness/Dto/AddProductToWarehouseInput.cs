using Entities.Enum.Warehouse;

namespace Bussiness.Dto
{
    public class AddProductToWarehouseInput
    {
        public string? ObjectName { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public EventType Type { get; set; }
    }
}
