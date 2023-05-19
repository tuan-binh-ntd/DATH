using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Photo : EntityDto
    {
        [Required, StringLength(500), DataType(DataType.Url)]
        public string? Url { get; set; }
        [Required, StringLength(100)]
        public string? PublicId { get; set; }
        public bool IsMain { get; set; }
        // Relationship
        public Product? Product { get; set; }
        public long ProductId { get; set; }
        public Specification? Specification { get; set; }
        public long? SpecificationId { get; set; }
    }
}
