using Entities;

namespace Bussiness.Dto
{
    public class ProductForViewDto : EntityDto<long>
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? SpecificationId { get; set; }
        public int? ProductCategoryId { get; set; }
        public decimal Star { get; set; }
        public ICollection<PhotoDto>? Photos { get; set; }
        public ICollection<SpecificationDto>? Specifications { get; set; }
    }
}
