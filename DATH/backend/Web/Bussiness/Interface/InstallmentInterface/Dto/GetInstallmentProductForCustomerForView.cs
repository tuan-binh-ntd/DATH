using Bussiness.Dto;
using Bussiness.Services.ProductService.Dto;
using Entities;

namespace Bussiness.Interface.InstallmentInterface.Dto
{
    public class GetInstallmentProductForCustomerForView : EntityDto<long>
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? SpecificationId { get; set; }
        public int? ProductCategoryId { get; set; }
        public decimal Star { get; set; }
        public ICollection<PhotoDto>? Photos { get; set; }
        public ICollection<SpecificationDto>? Specifications { get; set; }
        public string? OrderCode { get; set; }
        public int Term { get; set; }
        public decimal Money { get; set; }
        public ICollection<FeedbackDto>? Feedbacks { get; set; }
    }
}
