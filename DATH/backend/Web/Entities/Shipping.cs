using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Shipping : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [Column(TypeName = "decimal(19, 5)")]
        public decimal Cost { get; set; }

        // Relationship
        public ICollection<Order>? Orders { get; set; }
    }
}
