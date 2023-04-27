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
        private readonly IRepository<SpecificationCategory> _specCateRepo;
        private readonly IRepository<Product, long> _productRepo;
        private readonly IRepository<Specification, long> _specRepo;

        public ProductCategorysController(
            IMapper mapper,
            IRepository<ProductCategory> productCateRepo,
            IRepository<SpecificationCategory> specCateRepo,
            IRepository<Product, long> productRepo,
            IRepository<Specification, long> specRepo
            )
        {
            _mapper = mapper;
            _productCateRepo = productCateRepo;
            _specCateRepo = specCateRepo;
            _productRepo = productRepo;
            _specRepo = specRepo;
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

        [HttpGet("{id}/specifications")]
        public async Task<IActionResult> GetFilterForProductCategory(int id)
        {
            List<string?> specificationIdList = await _productRepo.GetAll().AsNoTracking().Where(p => p.ProductCategoryId == id).Select(p => p.SpecificationId).ToListAsync();

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
    }
}
