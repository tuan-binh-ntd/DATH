
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Shipping : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        public decimal Cost { get; set; }
    }
}
