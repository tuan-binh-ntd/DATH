using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Payment : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
    }
}
