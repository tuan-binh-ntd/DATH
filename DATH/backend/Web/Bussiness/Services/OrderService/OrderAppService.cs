using AutoMapper;
using Bussiness.Helper;
using Bussiness.Interface.Core;
using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Entities.Enum.Order;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.OrderService
{
    public class OrderAppService : BaseService, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepo;
        private readonly IRepository<OrderDetail, long> _orderDetailRepo;
        private readonly IDapper _dapper;

        public OrderAppService(
            IMapper mapper,
            IRepository<Order, long> orderRepo,
            IRepository<OrderDetail, long> orderDetailRepo,
            IDapper dapper
            )
        {
            ObjectMapper = mapper;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _dapper = dapper;
        }

        #region CreateOrder
        public async Task<OrderForViewDto> CreateOrder(OrderInput input)
        {
            Order order = ObjectMapper!.Map<Order>(input);
            order.Code = await GenerateOrderCode();
            order.Status = OrderStatus.Pending;

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
        #endregion

        #region ForwardToTheStore
        public async Task<object> ForwardToTheStore(long id, ForwardToTheStoreInput input)
        {
            Order? order = await _orderRepo.GetAsync(id);
            order!.ShopId = input.ShopId;

            await _orderRepo.UpdateAsync(order);
            return ObjectMapper!.Map<OrderForViewDto>(order);
        }
        #endregion

        #region GetOrdersForAdmin
        public async Task<object> GetOrdersForAdmin(PaginationInput input)
        {

            object res = await GetOrders(null, input);
            return res;
        }
        #endregion

        #region GetOrdersForShop
        public async Task<object> GetOrdersForShop(int shopId, PaginationInput input)
        {
            object res = await GetOrders(shopId, input);
            return res;
        }
        #endregion

        public Task<object> UpdateOrder()
        {
            throw new NotImplementedException();
        }

        #region GetOrder by Id 
        public async Task<OrderForViewDto?> GetOrder(long id)
        {
            IQueryable<OrderForViewDto> query = from o in _orderRepo.GetAll()
                                                 where o.Id == id
                                                 select new OrderForViewDto
                                                 {
                                                     Id = o.Id,
                                                     CustomerName = o.CustomerName,
                                                     Address = o.Address,
                                                     Phone = o.Phone,
                                                     Code = o.Code,
                                                     Status = o.Status,
                                                     ActualDate = o.ActualDate,
                                                     EstimateDate = o.EstimateDate,
                                                     Cost = o.Cost,
                                                     Discount = o.Discount,
                                                     CreateDate = (DateTime)o.CreationTime!
                                                 };
            OrderForViewDto? res = await query.SingleOrDefaultAsync();

            await HandleOrder(res!);

            return res;
        }
        #endregion

        #region GetOrder by Code
        public async Task<OrderForViewDto?> GetOrder(string code)
        {
            IQueryable<OrderForViewDto> query = from o in _orderRepo.GetAll()
                                                where o.Code == code
                                                select new OrderForViewDto
                                                {
                                                    Id = o.Id,
                                                    CustomerName = o.CustomerName,
                                                    Address = o.Address,
                                                    Phone = o.Phone,
                                                    Code = o.Code,
                                                    Status = o.Status,
                                                    ActualDate = o.ActualDate,
                                                    EstimateDate = o.EstimateDate,
                                                    Cost = o.Cost,
                                                    Discount = o.Discount,
                                                    CreateDate = (DateTime)o.CreationTime!
                                                };
            OrderForViewDto? res = await query.SingleOrDefaultAsync();

            await HandleOrder(res!);

            return res;
        }
        #endregion

        #region Private method

        #region HandleOrders
        private async Task HandleOrders(ICollection<OrderForViewDto> input)
        {
            foreach (OrderForViewDto item in input)
            {
                await HandleOrder(item);
            }
        }

        private async Task HandleOrder(OrderForViewDto input)
        {
            input.OrderDetails = await (from od in _orderDetailRepo.GetAll()
                                        where od.OrderId == input.Id
                                        select new OrderDetailForViewDto
                                        {
                                            Id = od.Id,
                                            Cost = od.Cost,
                                            Quantity = od.Quantity,
                                            SpecificationId = od.SpecificationId,
                                            ProductId = od.ProductId,
                                            InstallmentId = od.InstallmentId,
                                        }).ToListAsync();
        }
        #endregion

        #region GetOrders
        private async Task<object> GetOrders(int? shopId, PaginationInput input)
        {
            if(shopId is null)
            {
                IQueryable<OrderForViewDto> orders = from o in _orderRepo.GetAll()
                                                     where o.ShopId == null
                                                     select new OrderForViewDto
                                                     {
                                                         Id = o.Id,
                                                         CustomerName = o.CustomerName,
                                                         Address = o.Address,
                                                         Phone = o.Phone,
                                                         Code = o.Code,
                                                         Status = o.Status,
                                                         ActualDate = o.ActualDate,
                                                         EstimateDate = o.EstimateDate,
                                                         Cost = o.Cost,
                                                         Discount = o.Discount,
                                                         CreateDate = (DateTime)o.CreationTime!
                                                     };

                if (input.PageNum != null && input.PageSize != null)
                {
                    PaginationResult<OrderForViewDto> res = await orders.Pagination(input);
                    await HandleOrders(res.Content!);
                    return res;
                }
                else
                {
                    List<OrderForViewDto> res = await orders.ToListAsync();
                    await HandleOrders(res);
                    return res;
                }
            }
            else
            {
                IQueryable<OrderForViewDto> orders = from o in _orderRepo.GetAll()
                                                     where o.ShopId == shopId
                                                     select new OrderForViewDto
                                                     {
                                                         Id = o.Id,
                                                         CustomerName = o.CustomerName,
                                                         Address = o.Address,
                                                         Phone = o.Phone,
                                                         Code = o.Code,
                                                         Status = o.Status,
                                                         ActualDate = o.ActualDate,
                                                         EstimateDate = o.EstimateDate,
                                                         Cost = o.Cost,
                                                         Discount = o.Discount,
                                                         CreateDate = (DateTime)o.CreationTime!
                                                     };

                if (input.PageNum != null && input.PageSize != null)
                {
                    PaginationResult<OrderForViewDto> res = await orders.Pagination(input);
                    await HandleOrders(res.Content!);
                    return res;
                }
                else
                {
                    List<OrderForViewDto> res = await orders.ToListAsync();
                    await HandleOrders(res);
                    return res;
                }
            }
        }
        #endregion

        #region GenerateOrderCode
        private async Task<string> GenerateOrderCode()
        {
            string now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            string prefix = "TC";

            long orderCount = await _dapper.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM [Order]");
            orderCount++;

            return $"{prefix}{now}{orderCount:D6}";
        }
        #endregion

        #endregion
    }
}
