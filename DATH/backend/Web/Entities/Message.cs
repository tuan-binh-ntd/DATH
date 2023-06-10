using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Message : BaseEntity<long>
    {
        [Required, StringLength(40)]
        public string? SenderUserName { get; set; }
        [Required, StringLength(40)]
        public string? RecipientUserName { get; set; }
        [Required]
        public string? Content { get; set; }
        public DateTime? DateRead { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

        // Relationship
        public long? SenderId { get; set; }
        public AppUser? Sender { get; set; }
        public long? RecipientId { get; set; }
        public AppUser? Recipient { get; set; }
    }
}
