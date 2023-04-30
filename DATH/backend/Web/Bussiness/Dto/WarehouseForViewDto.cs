using Entities;

namespace Bussiness.Dto
{
    public class WarehouseForViewDto : EntityDto
    {
        public string? ShopName { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public int? ShopId { get; set; }
    }
}
