using Entities;

namespace Bussiness.Services.ProductService.Dto
{
    public class FeedbackDto : EntityDto<long>
    {
        public string? UserName { get; set; }
        public decimal Star { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
