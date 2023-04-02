using Entities;

namespace Service.DemoService.Dto
{
    public class DemoDto : EntityDto<long?>
    {
        public string? Name { get; set; }
    }
}
