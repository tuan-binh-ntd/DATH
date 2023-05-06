using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface;
using Bussiness.Repository;
using Bussiness.Services;
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

        public ProductsController(
            IMapper mapper,
            IRepository<Product, long> productRepo,
            IPhotoService photoService,
            IRepository<Specification, long> specificationRepo,
            DataContext dataContext
            )
        {
            _mapper = mapper;
            _productRepo = productRepo;
            _photoService = photoService;
            _specificationRepo = specificationRepo;
            _dataContext = dataContext;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<ProductForViewDto> query = from p in _productRepo.GetAll().AsNoTracking()
                                                  select new ProductForViewDto()
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                      Price = p.Price,
                                                      Description = p.Description,
                                                      ProductCategoryId = p.ProductCategoryId,
                                                      SpecificationId = p.SpecificationId,
                                                  };

            ICollection<ProductForViewDto> list = new List<ProductForViewDto>();

            if (input.PageNum != null && input.PageSize != null)
            {
                PaginationResult<ProductForViewDto> products = await query.Pagination(input);
                list = products.Content!;
                await HandleProductList(list);
                return CustomResult(products, HttpStatusCode.OK);
            }
            else
            {
                list = await query.ToListAsync();
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
                                                      SpecificationId = p.SpecificationId,
                                                  };
            ProductForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInput input)
        {
            Product product = new();
            _mapper.Map(input, product);
            await _productRepo.InsertAsync(product);

            ProductForViewDto? res = new();
            _mapper.Map(product, res);

            await HandleProduct(res);

            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductInput input)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return CustomResult(HttpStatusCode.NoContent);

            _mapper.Map(input, product);

            await _productRepo.UpdateAsync(product);

            ProductForViewDto? res = new();
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

        [HttpPost("{id}/photos")]
        public async Task<IActionResult> AddPhoto(long id, IFormFile file)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return CustomResult(HttpStatusCode.NoContent);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return CustomResult(result.Error.Message, HttpStatusCode.BadRequest);

            Photo? photo = new()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            product.Photos = new List<Photo> { photo };

            ProductForViewDto? res = new();
            _mapper.Map(product, res);

            await HandleProduct(res);

            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos!.Add(_mapper.Map<PhotoDto>(photo));
                return CustomResult(res, HttpStatusCode.OK);
            }
            return CustomResult("Problem adding photo", HttpStatusCode.BadRequest);
        }
    }
}
