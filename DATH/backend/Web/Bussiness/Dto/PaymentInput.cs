using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dto
{
    public class PaymentInput
    {
        [StringLength(100)]
        public string? Name { get; set; }
    }
}
