
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Shipping
    {
        [StringLength(100)]
        public string? ShippingType { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
