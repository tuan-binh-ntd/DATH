using System.ComponentModel.DataAnnotations;

namespace Bussiness.Interface.OrderInterface.Dto
{
    public class OrderInput
    {
        [StringLength(100)]
        public string? CustomerName { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(11)]
        public string? Phone { get; set; }
        [StringLength(100), Required]
        public string? Email { get; set; }
        public DateTime? EstimateDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public decimal Cost { get; set; }
        public decimal Discount { get; set; } = 0;
        public int? PromotionId { get; set; }
        public int PaymentId { get; set; }
        public ICollection<OrderDetailInput>? OrderDetailInputs { get; set; }
    }
}
