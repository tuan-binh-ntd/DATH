using Entities;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Interface.NotificationInterface.Dto
{
    public class NotificationForViewDto : EntityDto<long>
    {
        [Required]
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public long UserId { get; set; }
    }
}
