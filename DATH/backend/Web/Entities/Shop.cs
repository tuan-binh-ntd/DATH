using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Shop : BaseEntity
    {
        [StringLength(500)]
        public string? Name { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
