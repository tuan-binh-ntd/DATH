using Entities;

namespace Bussiness.Dto
{
    public class SpecificationDto : EntityDto<long>
    {
        public string? SpecificationCategoryCode { get; set; }
        public string? Code { get; set; }
        public string? Value { get; set; }
        public int? SpecificationCategoryId { get; set; }
    }
}
