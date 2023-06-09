using System.ComponentModel.DataAnnotations;

namespace Bussiness.Interface.FeedbackInterface.Dto
{
    public class FeedbackInput
    {
        [Required, StringLength(100)]
        public string? UserName { get; set; }
        [Required]
        public decimal Star { get; set; }
        [Required]
        public string? Comment { get; set; }
        public long ProductId { get; set; }
    }
}
