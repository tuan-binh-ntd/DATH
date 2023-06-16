using System.ComponentModel.DataAnnotations;

namespace Bussiness.Interface.OrderInterface.Dto
{
    public class OrderDetailInput
    {
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string? SpecificationId { get; set; }
        public long ProductId { get; set; }
        public int? InstallmentId { get; set; }
        public int? PaymentId { get; set; }
    }
}
