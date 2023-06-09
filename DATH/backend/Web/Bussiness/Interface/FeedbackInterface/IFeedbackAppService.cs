using Bussiness.Interface.FeedbackInterface.Dto;

namespace Bussiness.Interface.FeedbackInterface
{
    public interface IFeedbackAppService
    {
        Task<IEnumerable<FeedbackForViewDto>> Get(long productId);
        Task<FeedbackForViewDto> CreateOrUpdate(long? id, FeedbackInput input);
        Task Delete(long id);
    }
}
