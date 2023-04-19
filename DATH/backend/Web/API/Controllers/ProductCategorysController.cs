using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Repository;
using Bussiness.Services;
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
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            IQueryable<ProductCategoryForViewDto> query = from pc in _productCateRepo.GetAll().AsNoTracking()
                                                          select new ProductCategoryForViewDto()
                                                          {
                                                              Id = pc.Id,
                                                              Name = pc.Name,
                                                          };

            PaginationResult<ProductCategoryForViewDto> data = await query.Pagination(input);

            if (data == null) return CustomResult(HttpStatusCode.OK);
            return CustomResult(data, HttpStatusCode.OK);
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
            if(productCategory == null) return CustomResult(HttpStatusCode.NoContent);

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
    }
}
