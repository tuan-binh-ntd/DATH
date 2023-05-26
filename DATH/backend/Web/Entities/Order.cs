using Entities.Enum.Order;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Order : BaseEntity<long>
    {
        [StringLength(100)]
        public string? CustomerName { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? Code { get; set;}
        public OrderStatus Status { get; set; }
        [StringLength(11)]
        public string? Phone { get; set; }
        public DateTime? EstimateDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public decimal Cost { get; set; }
        public decimal Discount { get; set; } = 0;
        [StringLength(100), Required]
        public string? Email { get; set; }

        // Relationship
        public int? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        public int? ShippingId { get; set; }
        public Shipping? Shipping { get; set; }
        public long OrderDetailId { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public int? ShopId { get; set; }
        public Shop? Shop { get; set; }
    }
}
