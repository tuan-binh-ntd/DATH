using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class SpecificationCategory : BaseEntity
    {
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(100)]
        public string? Value { get; set; }
        public ICollection<Specification>? Specifications { get; set; }
    }
}
