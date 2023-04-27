using Entities;

namespace Bussiness.Dto
{
    public class WarehouseForViewDto : EntityDto
    {
        public string? Name { get; set; }
        public int? ParentId { get; set; }
    }
}
