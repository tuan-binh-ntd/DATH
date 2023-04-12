using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Product : BaseEntity<long>
    {
        [StringLength(500)]
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

        //Relationship
        public int? ProductCategoryId { get; set; }
        public ProductCategory? ProductCategory { get; set; }
        [StringLength(500)]
        public string? SpecificationId { get; set; }
        public Specification? Specification { get; set; }
    }
}
