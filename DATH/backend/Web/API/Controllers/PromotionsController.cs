using AutoMapper;
using Bussiness.Dto;
using Bussiness.Repository;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class PromotionsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Promotion> _promotionRepo;

        public PromotionsController(
            IMapper mapper,
            IRepository<Promotion> promotionRepo
            )
        {
            _mapper = mapper;
            _promotionRepo = promotionRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IQueryable<PromotionForViewDto> query = from p in _promotionRepo.GetAll().AsNoTracking()
                                                   select new PromotionForViewDto()
                                                   {
                                                       Id = p.Id,
                                                       Name = p.Name,
                                                       Code = p.Code,
                                                       StartDate = p.StartDate,
                                                       EndDate = p.EndDate,
                                                       Discount = p.Discount,
                                                   };
            List<PromotionForViewDto>? data = await query.ToListAsync();
            if (data == null) return CustomResult(HttpStatusCode.NotFound);

            return CustomResult(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IQueryable<PromotionForViewDto> query = from p in _promotionRepo.GetAll().AsNoTracking()
                                                   where p.Id == id
                                                   select new PromotionForViewDto()
                                                   {
                                                       Id = p.Id,
                                                       Name = p.Name,
                                                       Code = p.Code,
                                                       StartDate = p.StartDate,
                                                       EndDate = p.EndDate,
                                                       Discount = p.Discount,
                                                   };
            PromotionForViewDto? data = await query.FirstOrDefaultAsync();
            if (data == null) return CustomResult(null, HttpStatusCode.NotFound);

            return CustomResult(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShopInput input)
        {
            Promotion data = new();
            _mapper.Map(input, data);

            await _promotionRepo.InsertAsync(data);
            return CustomResult(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopInput input)
        {
            Promotion? data = await _promotionRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NotFound);
            _mapper.Map(input, data);

            await _promotionRepo.UpdateAsync(data);
            return CustomResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _promotionRepo.DeleteAsync(id);
            return CustomResult();
        }
    }
}
