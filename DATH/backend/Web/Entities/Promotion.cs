using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Promotion : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100), Unicode(false)]
        public string? Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Discount { get; set; }

        // Relationship
        public ICollection<Order>? Orders { get; set; }
    }
}
