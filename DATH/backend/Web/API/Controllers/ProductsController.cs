using Bussiness.Dto;
using Bussiness.Interface.FeedbackInterface;
using Bussiness.Interface.FeedbackInterface.Dto;
using Bussiness.Interface.ProductInterface;
using Bussiness.Services.Core;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    public class ProductsController : AdminBaseController
    {
        private readonly IProductAppService _productAppService;
        private readonly IFeedbackAppService _feedbackAppService;

        public ProductsController(
            IProductAppService productAppService,
            IFeedbackAppService feedbackAppService
            )
        {
            _productAppService = productAppService;
            _feedbackAppService = feedbackAppService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationInput input, [FromQuery] ProductFilterDto filter)
        {
            object res = await _productAppService.GetProducts(input, filter);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "Products")]
        public async Task<IActionResult> Get(long id)
        {
            ProductForViewDto? res = await _productAppService.GetProduct(id);
            if (res != null) return CustomResult(res, HttpStatusCode.OK);
            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInput input)
        {
            ProductForViewDto? res = await _productAppService.CreateOrUpdate(null, input);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductInput input)
        {
            ProductForViewDto? res = await _productAppService.CreateOrUpdate(id, input);
            if (res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _productAppService.Delete(id);
            return CustomResult(id, HttpStatusCode.OK);
        }

        [HttpPost("{id}/photos/{isMain}")]
        public async Task<IActionResult> AddPhoto(long id, bool isMain, [FromQuery] long? specificationId, IFormFile file)
        {
            object? res = await _productAppService.AddPhoto(id, isMain, specificationId, file);
            if (res == null)
            {
                return CustomResult(HttpStatusCode.NoContent);
            }
            else if (res is ProductForViewDto)
            {
                return CustomResult(res, HttpStatusCode.OK);
            }
            else if (res is string)
            {
                return CustomResult(res, HttpStatusCode.InternalServerError);
            }
            else
            {
                return CustomResult(res, HttpStatusCode.OK);
            }
        }

        [HttpDelete("{id}/photos/{photoId}")]
        public async Task<IActionResult> RemovePhoto(long id, int photoId)
        {
            object? res = await _productAppService.RemovePhoto(id, photoId);

            if (res == null)
            {
                return CustomResult(HttpStatusCode.NoContent);
            }
            else if (res is ProductForViewDto)
            {
                return CustomResult(HttpStatusCode.OK);
            }
            else if (res is string)
            {
                return CustomResult(res, HttpStatusCode.BadRequest);
            }
            else 
            { 
                return CustomResult(HttpStatusCode.BadRequest); 
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}&{specificationId}")]
        public async Task<IActionResult> GetProductBySpecificationId(long id, long specificationId)
        {
            ProductForViewDto? res = await _productAppService.GetProductBySpecificationId(id, specificationId);
            if(res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpGet("{id}/feedbacks")]
        public async Task<IActionResult> GetFeedbacks(long id)
        {
            IEnumerable<FeedbackForViewDto> res = await _feedbackAppService.Get(id);
            if (res == null) return CustomResult(HttpStatusCode.NoContent);
            return CustomResult(res, HttpStatusCode.OK);
        }
    }
}
