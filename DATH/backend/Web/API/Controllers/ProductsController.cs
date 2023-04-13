using AutoMapper;
using Bussiness.Dto;
using Bussiness.Extensions;
using Bussiness.Repository;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class ProductsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product, long> _productRepo;

        public ProductsController(
            IMapper mapper,
            IRepository<Product, long> productRepo
            )
        {
            _mapper = mapper;
            _productRepo = productRepo;
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

        [HttpGet("{id}")]
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
            Product product = new()
            {
                CreatorUserId = User.GetUserId()
            };

            _mapper.Map(input, product);

            await _productRepo.InsertAsync(product);

            return CustomResult(HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductInput input)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return CustomResult(HttpStatusCode.NotFound);
            product!.LastModifierUserId = User.GetUserId();

            _mapper.Map(input, product);

            await _productRepo.UpdateAsync(product);

            return CustomResult(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Product? product = await _productRepo.GetAsync(id);

            product!.DeleteUserId = User.GetUserId();

            await _productRepo.DeleteAsync(id);
            return CustomResult();
        }
    }
}
