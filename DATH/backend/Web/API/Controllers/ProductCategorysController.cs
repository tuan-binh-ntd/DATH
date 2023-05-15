using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface;
using Bussiness.Repository;
using Bussiness.Services;
using Dapper;
using Database;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class ProductCategorysController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ProductCategory> _productCateRepo;
        private readonly IRepository<SpecificationCategory> _specCateRepo;
        private readonly IRepository<Product, long> _productRepo;
        private readonly IRepository<Specification, long> _specRepo;
        private readonly IDapper _dapper;
        private readonly IRepository<Specification, long> _specificationRepo;
        private readonly DataContext _dataContext;

        public ProductCategorysController(
            IMapper mapper,
            IRepository<ProductCategory> productCateRepo,
            IRepository<SpecificationCategory> specCateRepo,
            IRepository<Product, long> productRepo,
            IRepository<Specification, long> specRepo,
            IDapper dapper,
            IRepository<Specification, long> specificationRepo,
            DataContext dataContext
            )
        {
            _mapper = mapper;
            _productCateRepo = productCateRepo;
            _specCateRepo = specCateRepo;
            _productRepo = productRepo;
            _specRepo = specRepo;
            _dapper = dapper;
            _specificationRepo = specificationRepo;
            _dataContext = dataContext;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<ProductCategoryForViewDto> query = from pc in _productCateRepo.GetAll().AsNoTracking()
                                                          select new ProductCategoryForViewDto()
                                                          {
                                                              Id = pc.Id,
                                                              Name = pc.Name,
                                                          };

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<ProductCategoryForViewDto> query = from pc in _productCateRepo.GetAll().AsNoTracking()
                                                          where pc.Id == id
                                                          select new ProductCategoryForViewDto()
                                                          {
                                                              Id = pc.Id,
                                                              Name = pc.Name,
                                                          };
            ProductCategoryForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryInput input)
        {
            ProductCategory productCategory = new();

            _mapper.Map(input, productCategory);

            await _productCateRepo.InsertAsync(productCategory);

            ProductCategoryForViewDto? res = new();
            _mapper.Map(productCategory, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCategoryInput input)
        {
            ProductCategory? productCategory = await _productCateRepo.GetAsync(id);
            if (productCategory == null) return CustomResult(HttpStatusCode.NoContent);

            _mapper.Map(input, productCategory);

            await _productCateRepo.UpdateAsync(productCategory);
            ProductCategoryForViewDto? res = new();
            _mapper.Map(productCategory, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productCateRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/specifications")]
        public async Task<IActionResult> GetFilterForProductCategory(int id)
        {
            List<string>? specificationIdList = await _productRepo.GetAll().AsNoTracking().Where(p => p.ProductCategoryId == id).Select(p => p.SpecificationId!.Substring(1)).ToListAsync();

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

            return CustomResult(list, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductByCategory(int id, [FromQuery] PaginationInput input, [FromQuery] ProductFilterDto filter)
        {
            DynamicParameters param = new();

            param.Add("ProductCategoryId", null);
            param.Add("SpecificationId", string.IsNullOrWhiteSpace(filter.SpecificationIds) ? "\"\"" : filter.SpecificationIds);
            param.Add("Price", filter.Price);
            param.Add("Keyword", string.IsNullOrWhiteSpace(filter.Keyword) ? "\"\"" : filter.Keyword);

            if (input.PageNum != null && input.PageSize != null)
            {
                PaginationResult<ProductForViewDto> products = await _dapper.GetAllAndPaginationAsync<ProductForViewDto>(@"
                    select 
			            Id,
			            [Name],
			            Price,
			            [Description],
			            ProductCategoryId,
			            substring(SpecificationId, 2, len(SpecificationId)) SpecificationId
		            from Product 
		            where IsDeleted = 0
			            and @ProductCategoryId is null or ProductCategoryId = @ProductCategoryId
			            and @SpecificationId is not null and freetext(SpecificationId, @SpecificationId) 
			            and @Keyword is not null and freetext([Name], @Keyword)
			            and @Keyword is null or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", input, param);

                await HandleProductList(products.Content!);
                return CustomResult(products, HttpStatusCode.OK);
            }
            else
            {
                List<ProductForViewDto> list = await _dapper.GetAllAsync<ProductForViewDto>("GetProduct", param);
                await HandleProductList(list);
                return CustomResult(list, HttpStatusCode.OK);
            }
        }

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
                                                    where specifications.Contains(s.Id.ToString())
                                                    select new SpecificationDto
                                                    {
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
                                        }).ToListAsync();
            }
        }
    }
}
