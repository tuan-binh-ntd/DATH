using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Product : BaseEntity<long>
    {
        [StringLength(500)]
        public string? Name { get; set; }
        [StringLength(500)]
        public string? AvatarUrl { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? SpecificationId { get; set; }

        //Relationship
        public int? ProductCategoryId { get; set; }
        public ProductCategory? ProductCategory { get; set; }
        public ICollection<Photo>? Photos { get; set; }
    }
}
