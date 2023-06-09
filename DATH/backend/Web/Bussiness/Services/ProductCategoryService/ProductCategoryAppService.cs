using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.Core;
using Bussiness.Interface.ProductCategoryInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Dapper;
using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bussiness.Services.ProductCategoryService
{
    public class ProductCategoryAppService : BaseService, IProductCategoryAppService
    {
        private readonly IRepository<ProductCategory> _productCateRepo;
        private readonly IRepository<SpecificationCategory> _specCateRepo;
        private readonly IRepository<Product, long> _productRepo;
        private readonly IRepository<Specification, long> _specRepo;
        private readonly IDapper _dapper;
        private readonly IRepository<Specification, long> _specificationRepo;
        private readonly DataContext _dataContext;
        private readonly IRepository<SpecificationCategory> _specificationCategoryRepo;

        public ProductCategoryAppService(
            IMapper mapper,
            IRepository<ProductCategory> productCateRepo,
            IRepository<SpecificationCategory> specCateRepo,
            IRepository<Product, long> productRepo,
            IRepository<Specification, long> specRepo,
            IDapper dapper,
            IRepository<Specification, long> specificationRepo,
            DataContext dataContext,
            IRepository<SpecificationCategory> specificationCategoryRepo
            )
        {
            ObjectMapper = mapper;
            _productCateRepo = productCateRepo;
            _specCateRepo = specCateRepo;
            _productRepo = productRepo;
            _specRepo = specRepo;
            _dapper = dapper;
            _specificationRepo = specificationRepo;
            _dataContext = dataContext;
            _specificationCategoryRepo = specificationCategoryRepo;
        }

        #region CreateOrUpdate
        public async Task<ProductCategoryForViewDto?> CreateOrUpdate(int? id, ProductCategoryInput input)
        {
            if (id == null)
            {
                ProductCategory productCategory = new();

                ObjectMapper!.Map(input, productCategory);
                await _productCateRepo.InsertAsync(productCategory);

                ProductCategoryForViewDto? res = new();
                ObjectMapper!.Map(productCategory, res);
                return res;
            }
            else
            {
                ProductCategory? productCategory = await _productCateRepo.GetAsync((int)id);
                if (productCategory == null) return null;

                ObjectMapper!.Map(input, productCategory);

                await _productCateRepo.UpdateAsync(productCategory);
                ProductCategoryForViewDto? res = new();
                ObjectMapper!.Map(productCategory, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            await _productCateRepo.DeleteAsync(id);
        }
        #endregion

        #region GetFilterForProductCategory
        public async Task<object> GetFilterForProductCategory(int id)
        {
            ICollection<int>? productCategorys = await _productCateRepo.GetAll().AsNoTracking().Where(pc => pc.ParentId == id).Select(pc => pc.Id).ToListAsync();


            List<string>? specificationIdList = await _productRepo.GetAll().AsNoTracking().Where(p => productCategorys.Contains((int)p.ProductCategoryId!)).Select(p => p.SpecificationId!.Substring(1)).ToListAsync();

            string specificationIds = string.Join(",", specificationIdList);

            specificationIdList = specificationIds.Split(",").ToList()!;

            List<SpecificationCategoryDto> specificationCategories = await (from sc in _specCateRepo.GetAll().AsNoTracking()
                                                                            join s in _specRepo.GetAll().AsNoTracking() on sc.Id equals s.SpecificationCategoryId
                                                                            where specificationIdList.Contains(s.Id.ToString())
                                                                            select new SpecificationCategoryDto
                                                                            {
                                                                                Id = sc.Id,
                                                                                Code = sc.Code,
                                                                                Value = sc.Value,
                                                                                SpecificationCode = s.Code,
                                                                                SpecificationValue = s.Value,
                                                                                SpecificationId = s.Id,
                                                                            }).ToListAsync();


            var list = specificationCategories
            .GroupBy(sc => new { sc.Id, sc.Code, sc.Value })
            .Select(sc => new
            {
                sc.Key.Id,
                sc.Key.Code,
                sc.Key.Value,
                Specifications = sc.GroupBy(sc => new
                {
                    Id = sc.SpecificationId,
                    Code = sc.SpecificationCode,
                    Value = sc.SpecificationValue,
                }).Select(s => new
                {
                    s.Key.Id,
                    s.Key.Code,
                    s.Key.Value,
                }).ToList()
            }).ToList();

            return list;
        }
        #endregion

        #region GetProductByCategory
        public async Task<object> GetProductByCategory(int id, PaginationInput input, ProductFilterDto filter)
        {
            DynamicParameters param = new();
            string? productIdString = string.Join(",", await _productCateRepo.GetAll().AsNoTracking().Where(pc => pc.ParentId == id).Select(pc => pc.Id).ToListAsync());

            param.Add("ProductCategoryId", productIdString);
            param.Add("SpecificationId", string.IsNullOrWhiteSpace(filter.SpecificationIds) ? @"null" : "," + filter.SpecificationIds);
            param.Add("Price", filter.Price);
            param.Add("Keyword", string.IsNullOrWhiteSpace(filter.Keyword) ? @"null" : filter.Keyword);

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
			            and @ProductCategoryId is null or ProductCategoryId in (select * from string_split(@ProductCategoryId, ','))
			            and (freetext(SpecificationId, @SpecificationId) or @SpecificationId = N'null')
			            and (freetext([Name], @Keyword) or @Keyword = N'null')
			            or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", input, param);

                await HandleProductList(products.Content!);
                return products;
            }
            else
            {
                List<ProductForViewDto> list = await _dapper.GetAllAsync<ProductForViewDto>(@$"
                    select 
			            Id,
			            [Name],
			            Price,
			            [Description],
			            ProductCategoryId,
			            substring(SpecificationId, 2, len(SpecificationId)) SpecificationId
		            from Product 
		            where IsDeleted = 0
			            and @ProductCategoryId is null or ProductCategoryId in (select * from string_split(@ProductCategoryId, ','))
			            and (freetext(SpecificationId, @SpecificationId) or @SpecificationId = 'null')
			            and (freetext([Name], @Keyword) or @Keyword = 'null')
			            or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", param, CommandType.Text);

                await HandleProductList(list);
                return list;
            }
        }
        #endregion

        #region GetProductCategory
        public async Task<ProductCategoryForViewDto?> GetProductCategory(int id)
        {
            IQueryable<ProductCategoryForViewDto> query = from pc in _productCateRepo.GetAll().AsNoTracking()
                                                          where pc.Id == id
                                                          select new ProductCategoryForViewDto()
                                                          {
                                                              Id = pc.Id,
                                                              Name = pc.Name,
                                                          };
            ProductCategoryForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return null;
            return data;
        }
        #endregion

        #region GetProductCategorys

        public async Task<object> GetProductCategorys(PaginationInput input)
        {
            IQueryable<ProductCategoryForViewDto> query = from pc in _productCateRepo.GetAll().AsNoTracking()
                                                          select new ProductCategoryForViewDto()
                                                          {
                                                              Id = pc.Id,
                                                              Name = pc.Name,
                                                          };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
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
            }
        }
        #endregion
    }
}
