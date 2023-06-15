using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class OrderDetail : BaseEntity<long>
    {
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required, StringLength(100)]
        public string? SpecificationId { get; set; }

        // Relationship
        public long OrderId { get; set; }
        public Order? Order { get; set; }
        public long ProductId { get; set; }
        public Product? Product { get; set; }
        public int? InstallmentId { get; set; }
        public Installment? Installment { get; set; }
        public ICollection<InstallmentSchedule>? InstallmentSchedules { get; set; }
        // Ignore property
        [NotMapped]
        public int PaymentId { get; set; }
    }
}
