using AutoMapper;
using Bussiness.Dto;
using Bussiness.Extensions;
using Bussiness.Interface;
using Bussiness.Repository;
using Entities;
using Microsoft.AspNetCore.Identity;
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

        public ProductsController(
            IMapper mapper,
            IRepository<Product, long> productRepo,
            IPhotoService photoService
            )
        {
            _mapper = mapper;
            _productRepo = productRepo;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IQueryable<ProductForViewDto> query = from p in _productRepo.GetAll().AsNoTracking()
                                                          select new ProductForViewDto()
                                                          {
                                                              Id = p.Id,
                                                              Name = p.Name,
                                                              Price = p.Price,
                                                              Description = p.Description
                                                          };
            List<ProductForViewDto> data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(data, HttpStatusCode.OK);
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
                                                      Description = p.Description
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

            return CustomResult(product, HttpStatusCode.OK);
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
            if(product == null) return CustomResult(HttpStatusCode.NoContent);

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


            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos = new List<PhotoDto> { _mapper.Map<PhotoDto>(photo) };
                return CustomResult(res, HttpStatusCode.OK);
            }
            return CustomResult("Problem adding photo", HttpStatusCode.BadRequest);
        }
    }
}
