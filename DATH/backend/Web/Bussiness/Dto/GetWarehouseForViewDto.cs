using Entities;

namespace Bussiness.Dto
{
    public class GetWarehouseForViewDto : EntityDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
    }
}
