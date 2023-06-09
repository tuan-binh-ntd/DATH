using Bussiness.Interface.FeedbackInterface;
using Bussiness.Interface.FeedbackInterface.Dto;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : BaseController
    {
        private readonly IFeedbackAppService _feedbackAppService;

        public FeedbacksController(
            IFeedbackAppService feedbackAppService
            )
        {
            _feedbackAppService = feedbackAppService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeedbackInput input)
        {
            FeedbackForViewDto res = await _feedbackAppService.CreateOrUpdate(null, input);
            return CustomResult(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Create(long id, FeedbackInput input)
        {
            FeedbackForViewDto res = await _feedbackAppService.CreateOrUpdate(id, input);
            return CustomResult(res);
        }
    }
}
