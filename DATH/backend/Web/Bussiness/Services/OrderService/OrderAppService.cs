﻿using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.Core;
using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Database;
using Entities;
using Entities.Enum.Order;
using Entities.Enum.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bussiness.Services.OrderService
{
    public class OrderAppService : BaseService, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepo;
        private readonly IRepository<OrderDetail, long> _orderDetailRepo;
        private readonly IDapper _dapper;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Product, long> _productRepo;
        private readonly DataContext _dataContext;
        private readonly IRepository<InstallmentSchedule, long> _installmentSchRepo;
        private readonly IRepository<Installment, int> _installmentRepo;

        public OrderAppService(
            IMapper mapper,
            IRepository<Order, long> orderRepo,
            IRepository<OrderDetail, long> orderDetailRepo,
            IDapper dapper,
            IRepository<Payment> paymentRepo,
            UserManager<AppUser> userManager,
            IRepository<Product, long> productRepo,
            DataContext dataContext,
            IRepository<InstallmentSchedule, long> installmentSchRepo,
            IRepository<Installment, int> installmentRepo
            )
        {
            ObjectMapper = mapper;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _dapper = dapper;
            _paymentRepo = paymentRepo;
            _userManager = userManager;
            _productRepo = productRepo;
            _dataContext = dataContext;
            _installmentSchRepo = installmentSchRepo;
            _installmentRepo = installmentRepo;
        }

        #region CreateOrder
        public async Task<OrderForViewDto> CreateOrder(OrderInput input)
        {
            Order order = ObjectMapper!.Map<Order>(input);
            order.Code = await GenerateOrderCode();
            order.Status = OrderStatus.Pending;
            order.IsExport = false;

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

            await CreateInstallment(orderDetails, input.OrderDetailInputs);

            return res;
        }
        #endregion

        #region ForwardToTheStore
        public async Task<OrderForViewDto> ForwardToTheStore(long id, UpdateOrderInput input)
        {
            Order? order = await _orderRepo.GetAsync(id);
            order!.ShopId = input.ShopId;

            await _orderRepo.UpdateAsync(order);
            OrderForViewDto? res = ObjectMapper!.Map<OrderForViewDto>(order);
            await HandleOrder(res);
            return res;
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

        #region UpdateOrder
        public async Task<OrderForViewDto> UpdateOrder(long id, UpdateOrderInput input)
        {
            Order? order = await _orderRepo.GetAsync(id);

            if (input.ShippingId is not null)
            {
                order!.ShippingId = input.ShippingId;
            }
            if (input.EstimateDate is not null)
            {
                order!.EstimateDate = input.EstimateDate;
            }

            order!.Status = input.Status switch
            {
                OrderStatus.Rejected => OrderStatus.Rejected,
                OrderStatus.Preparing => OrderStatus.Preparing,
                OrderStatus.Prepared => OrderStatus.Prepared,
                OrderStatus.Delivering => OrderStatus.Delivering,
                OrderStatus.Received => OrderStatus.Received,
                _ => order.Status
            };

            await _orderRepo.UpdateAsync(order);
            OrderForViewDto res = ObjectMapper!.Map<OrderForViewDto>(order);
            await HandleOrder(res);
            return res;
        }
        #endregion

        #region GetOrder by Id 
        public async Task<OrderForViewDto?> GetOrder(long id)
        {
            IQueryable<OrderForViewDto> query = from o in _orderRepo.GetAll().AsNoTracking()
                                                join p in _paymentRepo.GetAll().AsNoTracking() on o.PaymentId equals p.Id
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
                                                    CreateDate = (DateTime)o.CreationTime!,
                                                    Payment = p.Name
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
            input.OrderDetails = await (from od in _orderDetailRepo.GetAll().AsNoTracking()
                                        join p in _productRepo.GetAll().AsNoTracking() on od.ProductId equals p.Id
                                        where od.OrderId == input.Id
                                        select new OrderDetailForViewDto
                                        {
                                            Id = od.Id,
                                            Cost = od.Cost,
                                            Quantity = od.Quantity,
                                            SpecificationId = od.SpecificationId,
                                            ProductId = od.ProductId,
                                            InstallmentId = od.InstallmentId,
                                            ProductName = p.Name,
                                            Price = p.Price
                                        }).ToListAsync();

            await HandleOrderDetails(input.OrderDetails);
        }
        private async Task HandleOrderDetails(ICollection<OrderDetailForViewDto> input)
        {
            foreach (OrderDetailForViewDto item in input)
            {
                IQueryable<PhotoDto> query = from p in _dataContext.Photo.AsNoTracking().Where(p => p.ProductId == item.ProductId)
                                             select new PhotoDto
                                             {
                                                 Id = p.Id,
                                                 Url = p.Url,
                                                 IsMain = p.IsMain,
                                             };

                item.Photos = await query.ToListAsync();
            }

        }
        #endregion

        #region GetOrders
        private async Task<object> GetOrders(int? shopId, PaginationInput input)
        {
            if (shopId is null)
            {
                IQueryable<OrderForViewDto> orders = from o in _orderRepo.GetAll().AsNoTracking()
                                                     join p in _paymentRepo.GetAll().AsNoTracking() on o.PaymentId equals p.Id
                                                     where o.ShopId == null
                                                     orderby o.CreationTime descending
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
                                                         CreateDate = (DateTime)o.CreationTime!,
                                                         Payment = p.Name
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
                IQueryable<OrderForViewDto> orders = from o in _orderRepo.GetAll().AsNoTracking()
                                                     join p in _paymentRepo.GetAll().AsNoTracking() on o.PaymentId equals p.Id
                                                     where o.ShopId == shopId
                                                     orderby o.CreationTime descending
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
                                                         CreateDate = (DateTime)o.CreationTime!,
                                                         Payment = p.Name
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

            string prefix = "TS";

            long orderCount = await _dapper.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM [Order]");
            orderCount++;

            return $"{prefix}{now}{orderCount:D6}";
        }


        #endregion

        #endregion

        #region GetOrdersForCustomer
        public async Task<IEnumerable<OrderForViewDto>> GetOrdersForCustomer(long userId)
        {

            IQueryable<OrderForViewDto> query = from o in _orderRepo.GetAll().AsNoTracking()
                                                join p in _paymentRepo.GetAll().AsNoTracking() on o.PaymentId equals p.Id
                                                where o.CreatorUserId == userId
                                                orderby o.CreationTime descending
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
                                                    CreateDate = (DateTime)o.CreationTime!,
                                                    Payment = p.Name
                                                };

            List<OrderForViewDto> orders = await query.ToListAsync();

            await HandleOrders(orders);

            return orders;
        }
        #endregion

        #region CreateInstallment
        public async Task CreateInstallment(ICollection<OrderDetail> orderDetail, ICollection<OrderDetailInput> orderDetailInputs)
        {
            foreach(OrderDetail item in orderDetail)
            {
                if(item.InstallmentId is not null)
                {
                    ICollection<InstallmentSchedule> installmentSchedules = new List<InstallmentSchedule>();

                    Installment? installment = await _installmentRepo.GetAsync((int)item.InstallmentId);

                    decimal moneyPerTerm = ((item.Cost * installment!.Balance) / 100) / installment!.Term;

                    for(int i = 1; i <= installment!.Term; i++)
                    {

                        InstallmentSchedule installmentSchedule = new()
                        {
                            Term = i,
                            StartDate = DateTime.Now.AddMonths(i),
                            EndDate = DateTime.Now.AddMonths(i + 1),
                            Status = InstallmentStatus.Unpaid,
                            Money = moneyPerTerm,
                            OrderDetailId = item.Id,
                            PaymentId = item.PaymentId
                        };

                        installmentSchedules.Add(installmentSchedule);
                    }

                    await _installmentSchRepo.AddRangeAsync(installmentSchedules);
                }
            }
        }
        #endregion
    }
}
