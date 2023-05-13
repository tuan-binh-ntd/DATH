using Entities;
using Entities.Enum.Warehouse;

namespace Bussiness.Dto
{
    public class AddProductToParentWarehouseForViewDto
    {
        public string? ObjectName { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public EventType Type { get; set; }
        public DateTime ActualDate { get; set; }
    }
}
