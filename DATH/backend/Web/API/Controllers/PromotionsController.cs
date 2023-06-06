using Bussiness.Dto;
using Bussiness.Interface.PromotionInterface;
using Bussiness.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class PromotionsController : AdminBaseController
    {
        private readonly IPromotionAppService _promotionAppService;

        public PromotionsController(
            IPromotionAppService promotionAppService
            )
        {
            _promotionAppService = promotionAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input)
        {
            object res = await _promotionAppService.GetPromotions(input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            PromotionForViewDto? res = await _promotionAppService.GetPromotion(id);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }
        [AllowAnonymous]
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> Get(string code)
        {
            PromotionForViewDto? res = await _promotionAppService.GetPromotion(code);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PromotionInput input)
        {
            PromotionForViewDto? res = await _promotionAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PromotionInput input)
        {
            PromotionForViewDto? res = await _promotionAppService.CreateOrUpdate(id, input);
            if (res is null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _promotionAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }
    }
}
