using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Specification : BaseEntity<long>
    {
        [StringLength(50), Unicode(false)]
        public string? Code { get; set; }
        [StringLength(100)]
        public string? Value { get; set; }
        public string? Description { get; set; }

        //Relationship
        public int SpecificationCategoryId { get; set; }
        public SpecificationCategory? SpecificationCategory { get; set; }
        public ICollection<Photo>? Photos { get; set; }
    }
}
