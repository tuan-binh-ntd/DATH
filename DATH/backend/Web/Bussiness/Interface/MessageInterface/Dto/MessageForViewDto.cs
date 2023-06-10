using Entities;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Interface.MessageInterface.Dto
{
    public class MessageForViewDto : EntityDto<long>
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
        public long? SenderId { get; set; }
        public long? RecipientId { get; set; }
    }
}
