using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class ProductCategory : BaseEntity
    {
        [StringLength(1000)]
        public string? Name { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
