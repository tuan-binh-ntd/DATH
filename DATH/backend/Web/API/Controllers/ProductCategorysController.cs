using AutoMapper;
using Bussiness.Dto;
using Bussiness.Repository;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class ProductCategorysController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ProductCategory> _productCateRepo;

        public ProductCategorysController(
            IMapper mapper,
            IRepository<ProductCategory> productCateRepo
            )
        {
            _mapper = mapper;
            _productCateRepo = productCateRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IQueryable<ProductCategoryForViewDto> query = from pc in _productCateRepo.GetAll().AsNoTracking()
                                                          select new ProductCategoryForViewDto()
                                                          {
                                                              Id = pc.Id,
                                                              Name = pc.Name,
                                                          };
            List<ProductCategoryForViewDto> data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);
            return CustomResult(data);
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
            if (data == null) return CustomResult(HttpStatusCode.NotFound);
            return CustomResult(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryInput input)
        {
            ProductCategory productCategory = new();

            _mapper.Map(input, productCategory);

            await _productCateRepo.InsertAsync(productCategory);

            return CustomResult(HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCategoryInput input)
        {
            ProductCategory? productCategory = await _productCateRepo.GetAsync(id);
            if(productCategory == null) return CustomResult(HttpStatusCode.NotFound);

            _mapper.Map(input, productCategory);

            await _productCateRepo.UpdateAsync(productCategory);

            return CustomResult(productCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productCateRepo.DeleteAsync(id);
            return CustomResult();
        }
    }
}
