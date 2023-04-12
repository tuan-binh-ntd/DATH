using Entities;

namespace Bussiness.Dto
{
    public class SpecificationForViewDto : EntityDto<long>
    {
        public string? Code { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        public int SpecificationCategoryId { get; set; }
    }
}
