using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Demo : BaseEntity<long>
    {
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
