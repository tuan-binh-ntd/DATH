using AutoMapper;
using Bussiness.Interface.FeedbackInterface;
using Bussiness.Interface.FeedbackInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.FeedbackService
{
    public class FeedbackAppService : BaseService, IFeedbackAppService
    {
        private readonly IRepository<Feedback, long> _feedbackRepo;

        public FeedbackAppService(
            IMapper mapper,
            IRepository<Feedback, long> feedbackRepo
            )
        {
            ObjectMapper = mapper;
            _feedbackRepo = feedbackRepo;
        }

        public async Task<FeedbackForViewDto> CreateOrUpdate(long? id, FeedbackInput input)
        {
            if(id is null)
            {
                Feedback feedback = ObjectMapper!.Map<Feedback>(input);
                await _feedbackRepo.InsertAsync(feedback);
                return ObjectMapper!.Map<FeedbackForViewDto>(feedback);
            }
            else
            {
                Feedback? feedback = await _feedbackRepo.GetAsync((long)id);
                ObjectMapper!.Map(input, feedback);
                await _feedbackRepo.UpdateAsync(feedback!);
                return ObjectMapper!.Map<FeedbackForViewDto>(feedback);
            }
        }

        public async Task Delete(long id)
        {
            await _feedbackRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<FeedbackForViewDto>> Get(long productId)
        {
            IQueryable<FeedbackForViewDto> query = from f in _feedbackRepo.GetAll().AsNoTracking()
                                                   where f.ProductId == productId
                                                   select new FeedbackForViewDto
                                                   {
                                                       Id = f.Id,
                                                       Comment = f.Comment,
                                                       Star = f.Star,
                                                       UserName = f.UserName,
                                                   };

            return await query.ToListAsync();
        }
    }
}
