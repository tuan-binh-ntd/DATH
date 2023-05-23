using Entities;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dto
{
    public class PaymentForViewDto : EntityDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(500)]
        public string? Url { get; set; }
        [StringLength(100)]
        public string? PublicId { get; set; }
    }
}
