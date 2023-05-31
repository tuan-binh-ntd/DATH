using Entities.Enum.Order;

namespace Bussiness.Interface.OrderInterface.Dto
{
    public class UpdateOrderInput
    {
        public int? ShippingId { get; set; }
        public OrderStatus Status { get; set; }
        public int? ShopId { get; set; }
    }
}
