using Entities;

namespace Bussiness.Dto
{
    public class SpecificationCategoryDto : EntityDto
    {
        public string? Code { get; set; }
        public string? Value { get; set; }
        public string? SpecificationCode { get; set; }
        public string? SpecificationValue { get; set; }
        public long? SpecificationId { get; set; }
        public ICollection<SpecificationDto>? Specifications { get; set; }
    }
}
