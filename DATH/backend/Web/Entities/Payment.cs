using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Payment : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(500)]
        public string? Url { get; set; }
        [StringLength(100)]
        public string? PublicId { get; set; }
    }
}
