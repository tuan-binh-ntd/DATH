using Entities;

namespace Bussiness.Dto
{
    public class ShippingForViewDto : EntityDto
    {
        public string? Name { get; set; }
        public decimal Cost { get; set; }
    }
}
