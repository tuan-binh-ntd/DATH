using Entities;

namespace Bussiness.Dto
{
    public class ShopDto : EntityDto<int?>
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
