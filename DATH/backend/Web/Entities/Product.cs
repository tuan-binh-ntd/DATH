using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Product : BaseEntity<long>
    {
        [StringLength(500)]
        public string? Name { get; set; }
        [Column(TypeName = "decimal(19, 5)")]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? SpecificationId { get; set; }

        //Relationship
        public int? ProductCategoryId { get; set; }
        public ProductCategory? ProductCategory { get; set; }
        public ICollection<Photo>? Photos { get; set; }
        public ICollection<WarehouseDetail> Products { get; set; } = new List<WarehouseDetail>();
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
