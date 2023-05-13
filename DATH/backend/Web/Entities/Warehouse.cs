using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Warehouse : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        public int? ParentId { get; set; }

        //Relationship
        public Shop? Shop { get; set; }
        public int? ShopId { get; set; }
        public ICollection<WarehouseDetail> Products { get; set; } = new List<WarehouseDetail>();
    }
}
