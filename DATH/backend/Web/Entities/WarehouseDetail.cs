using Entities.Enum.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class WarehouseDetail : BaseEntity<long>
    {
        [StringLength(500)]
        public string? ObjectName { get; set; }
        public long ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateTime ActualDate { get; set; }
        public EventType Type { get; set; }
        public Warehouse? Warehouse { get; set; }
        public Product? Product { get; set; }
    }
}
