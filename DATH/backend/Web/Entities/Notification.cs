using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Notification : BaseEntity<long>
    {
        [Required]
        public string? Content { get; set; }
        public bool IsRead { get; set; }

        // Relationship
        public long UserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
