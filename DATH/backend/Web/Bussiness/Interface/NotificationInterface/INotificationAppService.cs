using Bussiness.Interface.NotificationInterface.Dto;

namespace Bussiness.Interface.NotificationInterface
{
    public interface INotificationAppService
    {
        Task<IEnumerable<NotificationForViewDto>> Get(long userId);
        Task<NotificationForViewDto> CreateOrUpdate(long? id, NotificationInput input);
        Task<int> UnreadNotificationNum(long userId);
    }
}
