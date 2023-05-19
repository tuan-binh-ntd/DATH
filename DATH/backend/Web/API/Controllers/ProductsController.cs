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
    public class ProductsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product, long> _productRepo;
        private readonly IPhotoService _photoService;
        private readonly IRepository<Specification, long> _specificationRepo;
        private readonly DataContext _dataContext;
        private readonly IDapper _dapper;
        private readonly IRepository<SpecificationCategory> _specificationCategoryRepo;

        public ProductsController(
            IMapper mapper,
            IRepository<Product, long> productRepo,
            IPhotoService photoService,
            IRepository<Specification, long> specificationRepo,
            DataContext dataContext,
            IDapper dapper,
            IRepository<SpecificationCategory> specificationCategoryRepo
            )
        {
            _mapper = mapper;
            _productRepo = productRepo;
            _photoService = photoService;
            _specificationRepo = specificationRepo;
            _dataContext = dataContext;
            _dapper = dapper;
            _specificationCategoryRepo = specificationCategoryRepo;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input, [FromQuery] ProductFilterDto filter)
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
                return CustomResult(products, HttpStatusCode.OK);
            }
            else
            {
                List<ProductForViewDto> query = await _dapper.GetAllAsync<ProductForViewDto>("GetProduct", param);
                await HandleProductList(query);
                return CustomResult(query, HttpStatusCode.OK);
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

        [AllowAnonymous]
        [HttpGet("{id}", Name = "Products")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<ProductForViewDto> query = from p in _productRepo.GetAll().AsNoTracking()
                                                  where p.Id == id
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
                return CustomResult(data, HttpStatusCode.OK);
            }
            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInput input)
        {
            Product product = new();
            input.SpecificationId = $",{input.SpecificationId}";
            _mapper.Map(input, product);
            await _productRepo.InsertAsync(product);

            ProductForViewDto? res = new();
            product.SpecificationId = product.SpecificationId![1..];
            _mapper.Map(product, res);

            if (input.File != null)
            {
            }
            await HandleProduct(res);

            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductInput input)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return CustomResult(HttpStatusCode.NoContent);
            input.SpecificationId = $",{input.SpecificationId}";
            _mapper.Map(input, product);

            await _productRepo.UpdateAsync(product);

            ProductForViewDto? res = new();
            product.SpecificationId = product.SpecificationId![1..];
            _mapper.Map(product, res);

            await HandleProduct(res);

            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [HttpPost("{id}/photos/{isMain}&{specificationId}")]
        public async Task<IActionResult> AddPhoto(long id, bool isMain, long specificationId, IFormFile file)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return CustomResult(HttpStatusCode.NoContent);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return CustomResult(result.Error.Message, HttpStatusCode.BadRequest);

            Photo? photo = new()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = isMain,
                SpecificationId = specificationId
            };

            product.Photos = new List<Photo> { photo };

            ProductForViewDto? res = new();
            _mapper.Map(product, res);

            await HandleProduct(res);

            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos!.Add(_mapper.Map<PhotoDto>(photo));
                if (isMain) return CustomResult(res, HttpStatusCode.OK);
                return CustomResult(photo.Url, HttpStatusCode.OK);
            }
            return CustomResult("Problem adding photo", HttpStatusCode.BadRequest);
        }

        [HttpDelete("{id}/photos/{photoId}")]
        public async Task<IActionResult> RemovePhoto(long id, int photoId)
        {
            Product? product = await _productRepo.GetAll().Where(p => p.Id == id).Include(p => p.Photos).FirstOrDefaultAsync();

            if (product == null) return CustomResult(HttpStatusCode.NoContent);

            Photo? photo = product.Photos!.SingleOrDefault(p => p.Id == photoId);
            if (photo == null) return CustomResult(HttpStatusCode.NotFound);

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return CustomResult(result.Error.Message, HttpStatusCode.BadRequest);
            }

            product.Photos!.Remove(photo);

            if (await _productRepo.UpdateAsync(product) != null)
            {
                return CustomResult(HttpStatusCode.OK);
            }
            return CustomResult("Failed to delete your photo", HttpStatusCode.BadRequest);
        }

        [AllowAnonymous]
        [HttpGet("{id}&{specificationId}")]
        public async Task<IActionResult> GetProductBySpecificationId(long id, long specificationId)
        {
            IQueryable<ProductForViewDto> query = from p in _productRepo.GetAll().AsNoTracking()
                                                  where p.Id == id && p.SpecificationId!.Contains(specificationId.ToString())
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

                return CustomResult(data, HttpStatusCode.OK);
            }
            return CustomResult(HttpStatusCode.NoContent);
        }
    }
}
