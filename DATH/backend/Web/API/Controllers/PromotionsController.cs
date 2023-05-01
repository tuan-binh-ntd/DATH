﻿using AutoMapper;
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
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
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

            if (input.PageNum != null && input.PageSize != null) return CustomResult(await query.Pagination(input), HttpStatusCode.OK);
            else return CustomResult(await query.ToListAsync(), HttpStatusCode.OK);
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
            if (data == null) return CustomResult(null, HttpStatusCode.NoContent);

            return CustomResult(data, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PromotionInput input)
        {
            Promotion data = new();
            _mapper.Map(input, data);

            await _promotionRepo.InsertAsync(data);

            PromotionForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PromotionInput input)
        {
            Promotion? data = await _promotionRepo.GetAsync(id);
            if (data == null) return CustomResult(HttpStatusCode.NoContent);
            _mapper.Map(input, data);

            await _promotionRepo.UpdateAsync(data);
            PromotionForViewDto? res = new();
            _mapper.Map(data, res);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _promotionRepo.DeleteAsync(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
