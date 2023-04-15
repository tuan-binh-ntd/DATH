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
                                                              AvatarUrl = p.AvatarUrl,
                                                              Price = p.Price,
                                                              Description = p.Description
                                                          };
            List<ProductForViewDto> data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);
            return CustomResult(data);
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
                                                      AvatarUrl = p.AvatarUrl,
                                                      Price = p.Price,
                                                      Description = p.Description
                                                  };
            ProductForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);
            return CustomResult(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInput input)
        {
            Product product = new();
            _mapper.Map(input, product);
            await _productRepo.InsertAsync(product);

            ProductForViewDto? res = new();
            _mapper.Map(product, res);

            return CustomResult(res, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductInput input)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return CustomResult(HttpStatusCode.NotFound);

            _mapper.Map(input, product);

            await _productRepo.UpdateAsync(product);

            ProductForViewDto? res = new();
            _mapper.Map(product, res);

            return CustomResult(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productRepo.DeleteAsync(id);
            return CustomResult();
        }

        [HttpPost("{id}/photos")]
        public async Task<IActionResult> AddPhoto(long id, IFormFile file)
        {
            Product? product = await _productRepo.GetAsync(id);
            if(product == null) return CustomResult(HttpStatusCode.NotFound);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            //if (product.Photos.Count == 0)
            //{
            //    photo.IsMain = true;
            //}

            product.Photos!.Add(photo);

            if (await _productRepo.UpdateAsync(product) != null)
            {
                return CreatedAtRoute("Products", new { username = product.Name }, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem adding photo");
        }
    }
}
