using Entities;

namespace Bussiness.Dto
{
    public class ShopForViewDto : EntityDto<int?>
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
