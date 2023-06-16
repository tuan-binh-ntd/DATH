using Entities.Enum.Order;

namespace Entities
{
    public class InstallmentSchedule : BaseEntity<long>
    {
        public int Term { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public InstallmentStatus Status { get; set; }
        public decimal Money { get; set; }

        // Relationship
        public long OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public int PaymentId { get; set; }
    }
}
