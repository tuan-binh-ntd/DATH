using Entities;
using Entities.Enum.Order;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Interface.OrderInterface.Dto
{
    public class OrderForViewDto : EntityDto<long>
    {
        [StringLength(100)]
        public string? CustomerName { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        public OrderStatus Status { get; set; }
        [StringLength(11)]
        public string? Phone { get; set; }
        public DateTime? EstimateDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public decimal Cost { get; set; }
        public decimal Discount { get; set; } = 0;
        public int? PromotionId { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<OrderDetailForViewDto>? OrderDetails { get; set; }
    }
}
