using AutoMapper;
using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;

namespace Bussiness.Services.OrderService
{
    public class OrderAppService : BaseService, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepo;
        private readonly IRepository<OrderDetail, long> _orderDetailRepo;

        public OrderAppService(
            IMapper mapper,
            IRepository<Order, long> orderRepo,
            IRepository<OrderDetail, long> orderDetailRepo
            )
        {
            ObjectMapper = mapper;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        public async Task<object> CreateOrder(OrderInput input)
        {
            Order order = ObjectMapper!.Map<Order>(input);
            long orderId = await _orderRepo.InsertAndGetIdAsync(order);

            ICollection<OrderDetail> orderDetails = new List<OrderDetail>();

            foreach (OrderDetailInput item in input.OrderDetailInputs!)
            {
                OrderDetail orderDetail = new()
                {
                    OrderId = orderId,
                };
                ObjectMapper!.Map(item, orderDetail);
                orderDetails.Add(orderDetail);
            }

            await _orderDetailRepo.AddRangeAsync(orderDetails);

            OrderForViewDto res = new();
            ObjectMapper!.Map(order, res);
            res.OrderDetails = new List<OrderDetailForViewDto>();

            foreach (OrderDetail item in orderDetails)
            {
                res.OrderDetails!.Add(ObjectMapper!.Map<OrderDetailForViewDto>(item));
            }

            return res;
        }

        public Task<object> GetOrders(PaginationInput input)
        {
            throw new NotImplementedException();
        }

        public Task<object> UpdateOrder()
        {
            throw new NotImplementedException();
        }
    }
}
