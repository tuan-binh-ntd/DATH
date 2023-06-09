using Bussiness.Interface.NotificationInterface.Dto;

namespace Bussiness.Interface.NotificationInterface
{
    public interface INotificationAppService
    {
        Task<IEnumerable<NotificationForViewDto>> Get(long userId);
        Task CreateOrUpdate(long? id, NotificationInput input);
        Task<int> UnReadNotificationNum(long userId);
    }
}
