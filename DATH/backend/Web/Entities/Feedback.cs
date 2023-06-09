using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Feedback : BaseEntity<long>
    {
        [Required, StringLength(100)]
        public string? UserName { get; set; }
        [Required]
        public decimal Star { get; set; }
        [Required]
        public string? Comment { get; set; }

        // Relationship
        public long ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
