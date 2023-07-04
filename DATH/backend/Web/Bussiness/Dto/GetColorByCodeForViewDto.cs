using Entities;

namespace Bussiness.Dto
{
    public class GetColorByCodeForViewDto : EntityDto<long>
    {
        public string? Code { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
    }
}
