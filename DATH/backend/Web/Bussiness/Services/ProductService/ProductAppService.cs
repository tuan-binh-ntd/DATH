using AutoMapper;
using Bussiness.Dto;
using Bussiness.Interface.Core;
using Bussiness.Interface.InstallmentInterface.Dto;
using Bussiness.Interface.ProductInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Dapper;
using Database;
using Entities;
using Entities.Enum.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace Bussiness.Services.ProductService
{
    public class ProductAppService : BaseService, IProductAppService
    {
        private readonly IRepository<Product, long> _productRepo;
        private readonly IPhotoService _photoService;
        private readonly IRepository<Specification, long> _specificationRepo;
        private readonly DataContext _dataContext;
        private readonly IDapper _dapper;
        private readonly IRepository<SpecificationCategory> _specificationCategoryRepo;
        private readonly IRepository<OrderDetail, long> _orderDetailRepo;
        private readonly IRepository<InstallmentSchedule, long> _installmentScheRepo;
        private readonly IRepository<Order, long> _orderRepo;
        private readonly IRepository<Feedback, long> _feedBackRepo;

        public ProductAppService(
            IRepository<Product, long> productRepo,
            IPhotoService photoService,
            IRepository<Specification, long> specificationRepo,
            DataContext dataContext,
            IDapper dapper,
            IRepository<SpecificationCategory> specificationCategoryRepo,
            IMapper mapper,
            IRepository<OrderDetail, long> orderDetailRepo,
            IRepository<InstallmentSchedule, long> installmentScheRepo,
            IRepository<Order, long> orderRepo,
            IRepository<Feedback, long> feedBackRepo
            )
        {
            _productRepo = productRepo;
            _photoService = photoService;
            _specificationRepo = specificationRepo;
            _dataContext = dataContext;
            _dapper = dapper;
            _specificationCategoryRepo = specificationCategoryRepo;
            _orderDetailRepo = orderDetailRepo;
            _installmentScheRepo = installmentScheRepo;
            _orderRepo = orderRepo;
            _feedBackRepo = feedBackRepo;
            ObjectMapper = mapper;
        }

        #region AddPhoto
        public async Task<object?> AddPhoto(long id, bool isMain, long? specificationId, IFormFile file)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return null;

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return result.Error.Message;

            Photo? photo = new()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = isMain,
                SpecificationId = specificationId
            };

            product.Photos = new List<Photo> { photo };

            ProductForViewDto? res = new();
            ObjectMapper!.Map(product, res);

            await HandleProduct(res);

            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos!.Add(ObjectMapper!.Map<PhotoDto>(photo));
                return res;
            }
            return "Problem adding photo";
        }
        #endregion

        #region CreateOrUpdate
        public async Task<ProductForViewDto?> CreateOrUpdate(long? id, ProductInput input)
        {
            if (id == null)
            {
                Product product = new();
                input.SpecificationId = $",{input.SpecificationId}";
                ObjectMapper!.Map(input, product);
                await _productRepo.InsertAsync(product);

                ProductForViewDto? res = new();
                product.SpecificationId = product.SpecificationId![1..];
                ObjectMapper!.Map(product, res);

                await HandleProduct(res);

                return res;
            }
            else
            {
                Product? product = await _productRepo.GetAsync((long)id);
                if (product == null) return null;
                input.SpecificationId = $",{input.SpecificationId}";
                ObjectMapper!.Map(input, product);

                await _productRepo.UpdateAsync(product);

                ProductForViewDto? res = new();
                product.SpecificationId = product.SpecificationId![1..];
                ObjectMapper.Map(product, res);

                await HandleProduct(res);

                return res;
            }
        }
        #endregion

        #region DeleteProduct
        public async Task<long> Delete(long id)
        {
            await _productRepo.DeleteAsync(id);
            return id;
        }
        #endregion

        #region GetProduct
        public async Task<ProductForViewDto?> GetProduct(long id)
        {
            IQueryable<ProductForViewDto> query = from p in _productRepo.GetAll().AsNoTracking()
                                                  where p.Id == id
                                                  orderby p.Price descending
                                                  select new ProductForViewDto()
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                      Price = p.Price,
                                                      Description = p.Description,
                                                      ProductCategoryId = p.ProductCategoryId,
                                                      SpecificationId = p.SpecificationId!.Substring(1),
                                                  };
            ProductForViewDto? data = await query.FirstOrDefaultAsync();
            if (data != null)
            {
                await HandleProduct(data);
                return data;
            }
            return null;
        }
        #endregion

        #region GetProductBySpecificationId
        public async Task<ProductForViewDto?> GetProductBySpecificationId(long id, long specificationId)
        {
            IQueryable<ProductForViewDto?> query = from p in _productRepo.GetAll().AsNoTracking()
                                                   where p.Id == id && p.SpecificationId!.Contains(specificationId.ToString())
                                                   orderby p.Price descending
                                                   select new ProductForViewDto()
                                                   {
                                                       Id = p.Id,
                                                       Name = p.Name,
                                                       Price = p.Price,
                                                       Description = p.Description,
                                                       ProductCategoryId = p.ProductCategoryId,
                                                       SpecificationId = p.SpecificationId!.Substring(1),
                                                   };
            ProductForViewDto? data = await query.SingleOrDefaultAsync();

            if (data != null)
            {
                // Get specification list for product
                data.Specifications = await (from s in _specificationRepo.GetAll().AsNoTracking()
                                             join sc in _specificationCategoryRepo.GetAll().AsNoTracking() on s.SpecificationCategoryId equals sc.Id
                                             where s.Id == specificationId
                                             select new SpecificationDto
                                             {
                                                 SpecificationCategoryId = sc.Id,
                                                 SpecificationCategoryCode = sc.Code,
                                                 Id = s.Id,
                                                 Code = s.Code,
                                                 Value = s.Value
                                             }).ToListAsync();


                // Get photo list for product
                data.Photos = await (from p in _dataContext.Photo.AsNoTracking()
                                     where p.ProductId == data.Id
                                     select new PhotoDto
                                     {
                                         Id = p.Id,
                                         Url = p.Url,
                                         IsMain = p.IsMain,
                                     }).ToListAsync();

                return data;
            }
            return null;
        }
        #endregion

        #region GetProducts
        public async Task<object> GetProducts(PaginationInput input, ProductFilterDto filter)
        {
            DynamicParameters param = new();

            param.Add("ProductCategoryId", null);
            param.Add("SpecificationId", string.IsNullOrWhiteSpace(filter.SpecificationIds) ? @"""" : filter.SpecificationIds);
            param.Add("Price", filter.Price);
            param.Add("Keyword", string.IsNullOrWhiteSpace(filter.Keyword) ? @"""" : filter.Keyword);

            if (input.PageNum != null && input.PageSize != null)
            {
                PaginationResult<ProductForViewDto> products = await _dapper.GetAllAndPaginationAsync<ProductForViewDto>(@$"
                    select 
			            Id,
			            [Name],
			            Price,
			            [Description],
			            ProductCategoryId,
			            substring(SpecificationId, 2, len(SpecificationId)) SpecificationId
		            from Product 
		            where IsDeleted = 0
			            and (freetext(SpecificationId, @SpecificationId) or @SpecificationId = '""')
			            and (freetext([Name], @Keyword) or @Keyword = '""')
			            or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", input, param);

                await HandleProductList(products.Content!);
                return products;
            }
            else
            {
                List<ProductForViewDto> query = await _dapper.GetAllAsync<ProductForViewDto>(@$"
                    select 
			            Id,
			            [Name],
			            Price,
			            [Description],
			            ProductCategoryId,
			            substring(SpecificationId, 2, len(SpecificationId)) SpecificationId
		            from Product 
		            where IsDeleted = 0
			            and (freetext(SpecificationId, @SpecificationId) or @SpecificationId = '""')
			            and (freetext([Name], @Keyword) or @Keyword = '""')
			            or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", param, CommandType.Text);
                await HandleProductList(query);
                return query;
            }
        }
        #endregion

        #region RemovePhoto
        public async Task<object?> RemovePhoto(long id, int photoId)
        {
            Product? product = await _productRepo.GetAll().Where(p => p.Id == id).Include(p => p.Photos).FirstOrDefaultAsync();

            if (product == null) return null;

            Photo? photo = product.Photos!.SingleOrDefault(p => p.Id == photoId);
            if (photo == null) return null;

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return result.Error.Message;
            }

            product.Photos!.Remove(photo);

            ProductForViewDto? res = new();
            ObjectMapper!.Map(product, res);

            await HandleProduct(res);


            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos!.Add(ObjectMapper!.Map<PhotoDto>(photo));
                return res;
            }
            return "Failed to delete your photo";
        }
        #endregion

        #region Private method
        private async Task HandleProductList(ICollection<ProductForViewDto> list)
        {
            if (list != null)
            {
                foreach (ProductForViewDto product in list!)
                {
                    await HandleProduct(product);
                }
            }
        }

        private async Task HandleProduct(ProductForViewDto product)
        {
            if (product != null)
            {
                // Get specification list for product
                List<string>? specifications = product.SpecificationId != null ? product.SpecificationId!.Split(",").ToList() : null;
                if (specifications != null)
                {
                    product.Specifications = await (from s in _specificationRepo.GetAll().AsNoTracking()
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

                // Get photo list for product
                product.Photos = await (from p in _dataContext.Photo.AsNoTracking()
                                        where p.ProductId == product.Id
                                        select new PhotoDto
                                        {
                                            Id = p.Id,
                                            Url = p.Url,
                                            IsMain = p.IsMain,
                                        }).ToListAsync();

                // Calc star
                product.Star = await _feedBackRepo.GetAll().AsNoTracking().Where(p => p.ProductId == product.Id).GroupBy(g => g.ProductId).Select(f => f.Average(g => g.Star)).FirstOrDefaultAsync();
            }
        }

        private async Task HandleProductList(ICollection<GetInstallmentProductForCustomerForView> list)
        {
            if (list != null)
            {
                foreach (GetInstallmentProductForCustomerForView product in list!)
                {
                    await HandleProduct(product);
                }
            }
        }

        private async Task HandleProduct(GetInstallmentProductForCustomerForView product)
        {
            if (product != null)
            {
                // Get specification list for product
                List<string>? specifications = product.SpecificationId != null ? product.SpecificationId!.Split(",").ToList() : null;
                if (specifications != null)
                {
                    product.Specifications = await (from s in _specificationRepo.GetAll().AsNoTracking()
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

                // Get photo list for product
                product.Photos = await (from p in _dataContext.Photo.AsNoTracking()
                                        where p.ProductId == product.Id
                                        select new PhotoDto
                                        {
                                            Id = p.Id,
                                            Url = p.Url,
                                            IsMain = p.IsMain,
                                        }).ToListAsync();

                product.Star = await _feedBackRepo.GetAll().AsNoTracking().GroupBy(g => new { g.ProductId, g.Star }).Select(f => f.Average(g => g.Star)).FirstOrDefaultAsync();
            }
        }
        #endregion

        #region GetInstallmentProductForCustomer
        public async Task<IEnumerable<GetInstallmentProductForCustomerForView>?> GetInstallmentProductForCustomer(long customerId)
        {
            IQueryable<GetInstallmentProductForCustomerForView> query = from p in _productRepo.GetAll().AsNoTracking()
                                                                        join od in _orderDetailRepo.GetAll().AsNoTracking() on p.Id equals od.ProductId
                                                                        join o in _orderRepo.GetAll().AsNoTracking() on od.OrderId equals o.Id
                                                                        join ins in _installmentScheRepo.GetAll().AsNoTracking() on od.Id equals ins.OrderDetailId
                                                                        where o.CreatorUserId == customerId && ins.Status == InstallmentStatus.Unpaid
                                                                        group ins by new { p.Id, p.Name, p.Price, od.SpecificationId, p.ProductCategoryId, o.Code } into gp
                                                                        select new GetInstallmentProductForCustomerForView
                                                                        {
                                                                            Id = gp.Key.Id,
                                                                            Name = gp.Key.Name,
                                                                            Price = gp.Key.Price,
                                                                            SpecificationId = gp.Key.SpecificationId,
                                                                            OrderCode = gp.Key.Code,
                                                                            Term = gp.Count(),
                                                                            Money = gp.Sum(p => p.Money),
                                                                        };

            List<GetInstallmentProductForCustomerForView> products = await query.ToListAsync();

            if (products != null)
            {
                await HandleProductList(products);
                return products;
            }
            return null;
        }
        #endregion
    }
}
