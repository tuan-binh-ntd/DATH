using API.Entities;

namespace API.Dto
{
    public class DemoDto : EntityDto<long?>
    {
        public string? Name { get; set; }
    }
        
}
