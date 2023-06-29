using AutoMapper;
using Bussiness.Dto;
using Bussiness.EmailService;
using Bussiness.Helper;
using Bussiness.Interface.Core;
using Bussiness.Interface.OrderInterface;
using Bussiness.Interface.OrderInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Database;
using Entities;
using Entities.Enum.Order;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using System.Globalization;

namespace Bussiness.Services.OrderService
{
    public class OrderAppService : BaseService, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepo;
        private readonly IRepository<OrderDetail, long> _orderDetailRepo;
        private readonly IDapper _dapper;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Product, long> _productRepo;
        private readonly DataContext _dataContext;
        private readonly IRepository<InstallmentSchedule, long> _installmentSchRepo;
        private readonly IRepository<Installment, int> _installmentRepo;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Specification, long> _specificationRepo;
        private readonly IRepository<SpecificationCategory> _specificationCategoryRepo;

        public OrderAppService(
            IMapper mapper,
            IRepository<Order, long> orderRepo,
            IRepository<OrderDetail, long> orderDetailRepo,
            IDapper dapper,
            IRepository<Payment> paymentRepo,
            IRepository<Product, long> productRepo,
            DataContext dataContext,
            IRepository<InstallmentSchedule, long> installmentSchRepo,
            IRepository<Installment, int> installmentRepo,
            IEmailSender emailSender,
            IRepository<Specification, long> specificationRepo,
            IRepository<SpecificationCategory> specificationCategoryRepo
            )
        {
            ObjectMapper = mapper;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _dapper = dapper;
            _paymentRepo = paymentRepo;
            _productRepo = productRepo;
            _dataContext = dataContext;
            _installmentSchRepo = installmentSchRepo;
            _installmentRepo = installmentRepo;
            _emailSender = emailSender;
            _specificationRepo = specificationRepo;
            _specificationCategoryRepo = specificationCategoryRepo;
        }

        #region CreateOrder
        public async Task<OrderForViewDto> CreateOrder(OrderInput input)
        {
            Order order = ObjectMapper!.Map<Order>(input);
            order.Code = await GenerateOrderCode();
            order.Status = input.ShopId is null ? OrderStatus.Pending : OrderStatus.Received;
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

            OrderForViewDto res = new()
            {
                CreateDate = (DateTime)order.CreationTime!,
            };
            ObjectMapper!.Map(order, res);
            res.Payment = await _paymentRepo.GetAll().AsNoTracking().Where(p => p.Id == order.PaymentId).Select(p => p.Name).FirstOrDefaultAsync();
            res.OrderDetails = new List<OrderDetailForViewDto>();

            foreach (OrderDetail item in orderDetails)
            {
                res.OrderDetails!.Add(ObjectMapper!.Map<OrderDetailForViewDto>(item));
            }
            // create installment schedule when InstallmentId in orderdetail is not null
            await CreateInstallment(orderDetails, input.OrderDetailInputs);
            // send email for customer
            await SendEmail(order);
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
            res.CreateDate = (DateTime)order.CreationTime!;
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
                                                    Email = o.Email,
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
                                            Price = p.Price,
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

                List<string>? specifications = item.SpecificationId != null ? item.SpecificationId!.Split(",").ToList() : null;
                if (specifications != null)
                {
                    item.Specifications = await (from s in _specificationRepo.GetAll().AsNoTracking()
                                                 join sc in _specificationCategoryRepo.GetAll().AsNoTracking() on s.SpecificationCategoryId equals sc.Id
                                                 where specifications.Contains(s.Id.ToString())
                                                 select new SpecificationDto
                                                 {
                                                     SpecificationCategoryId = sc.Id,
                                                     SpecificationCategoryCode = sc.Code,
                                                     Id = s.Id,
                                                     Code = s.Code,
                                                     Value = s.Value
                                                 }).ToListAsync();
                }
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
                                                         Email = o.Email,
                                                         Cost = o.Cost,
                                                         Discount = o.Discount,
                                                         CreateDate = (DateTime)o.CreationTime!,
                                                         CreatorUserId = o.CreatorUserId,
                                                         ShopId = o.ShopId,
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
                                                         CreatorUserId = o.CreatorUserId,
                                                         ShopId = o.ShopId,
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

        #region Send email to customer
        private async Task SendEmail(Order input)
        {
            string head = @"
                <!DOCTYPE html>
                <html>
                <head>
                  <title>Order Confirmation</title>
                  <style>
                    body {
                      font-family: Arial, sans-serif;
                    }

                    .container {
                      width: 500px;
                      margin: 0 auto;
                      padding: 20px;
                      background-color: #f5f5f5;
                      border: 1px solid #ccc;
                      border-radius: 4px;
                    }

                    h1 {
                      text-align: center;
                      color: #333;
                    }

                    p {
                      margin-bottom: 20px;
                      line-height: 1.5;
                    }

                    .button {
                      display: inline-block;
                      padding: 10px 20px;
                      background-color: #fca311;
                      color: #242525;
                      text-decoration: none;
                      border-radius: 4px;
                    }
                  </style>
                </head>
            ";


            EmailMessage message = new(new string[] { input.Email! }, "Order Confirmation", head + $@"
                <body>
                  <div class=""container"">
                    <h1>Order Confirmation</h1>
                    <p>Dear {input.CustomerName} </p>
                    <p>Thank you for your order! We are pleased to inform you that your order has been successfully placed and will be processed shortly.</p>
                    <p>Order Details:</p>
                    <ul>
                      <li>Order Code: {input.Code} </li>
                      <li>Order Date: {input.CreationTime} </li>
                      <li>Order Cost: {input.Cost.ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat)} </li>
                    </ul>
                    <p>If you have any questions or need further assistance, please don't hesitate to contact our customer support.</p>
                    <p>Thank you for choosing our services!</p>
                    <p>Sincerely,</p>
                    <p>The [Company Name] Team</p>
                    <div style=""text-align: center;"">
                      <a href=""[Company Website]"" class=""button"">Visit Our Website</a>
                    </div>
                  </div>
                </body>
                </html>
            ");

            await _emailSender.SendEmailAsync(message, TextFormat.Html);
        }
        #endregion

        #region CreateInstallment
        private async Task CreateInstallment(ICollection<OrderDetail> orderDetail, ICollection<OrderDetailInput> orderDetailInputs)
        {
            foreach (OrderDetail item in orderDetail)
            {
                if (item.InstallmentId is not null)
                {
                    ICollection<InstallmentSchedule> installmentSchedules = new List<InstallmentSchedule>();

                    Installment? installment = await _installmentRepo.GetAsync((int)item.InstallmentId);

                    decimal productPrice = await _productRepo.GetAll().AsNoTracking().Where(e => e.Id == item.ProductId).Select(e => e.Price).SingleOrDefaultAsync();

                    decimal moneyPerTerm = ((productPrice * installment!.Balance + productPrice * installment!.Interest)  / 100) / installment!.Term;

                    for (int i = 1; i <= installment!.Term; i++)
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
                                                    CreatorUserId = o.CreatorUserId,
                                                    Payment = p.Name
                                                };

            List<OrderForViewDto> orders = await query.ToListAsync();

            await HandleOrders(orders);

            return orders;
        }
        #endregion

        #region CustomerRecievedOrder

        #endregion
        public async Task<OrderForViewDto> CustomerRecievedOrder(long orderId)
        {
            Order? order = await _orderRepo.GetAll().AsNoTracking().Where(o => o.Id == orderId).SingleOrDefaultAsync();

            order!.Status = OrderStatus.Received;

            await _orderRepo.UpdateAsync(order);

            IQueryable<OrderForViewDto> query = from o in _orderRepo.GetAll().AsNoTracking()
                                                join p in _paymentRepo.GetAll().AsNoTracking() on o.PaymentId equals p.Id
                                                where o.Id == orderId
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

            return res!;
        }
    }
}
