using AutoMapper;
using Bussiness.Interface.NotificationInterface;
using Bussiness.Interface.NotificationInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.NotificationService
{
    public class NotificationAppService : BaseService, INotificationAppService
    {
        private readonly IRepository<Notification, long> _notificationRepo;

        public NotificationAppService(
            IMapper mapper,
            IRepository<Notification, long> notificationRepo
            )
        {
            ObjectMapper = mapper;
            _notificationRepo = notificationRepo;
        }

        #region CreateOrUpdate
        public async Task<NotificationForViewDto> CreateOrUpdate(long? id, NotificationInput input)
        {
            if (id is null)
            {
                Notification notification = ObjectMapper!.Map<Notification>(input);
                await _notificationRepo.InsertAsync(notification);
                NotificationForViewDto res = ObjectMapper!.Map<NotificationForViewDto>(notification);
                return res;
            }
            else
            {
                Notification? notification = await _notificationRepo.GetAsync((long)id);
                ObjectMapper!.Map(input, notification);
                await _notificationRepo.UpdateAsync(notification!);
                NotificationForViewDto res = ObjectMapper!.Map<NotificationForViewDto>(notification);
                return res;
            }
        }
        #endregion

        #region Get
        public async Task<IEnumerable<NotificationForViewDto>> Get(long userId)
        {
            IQueryable<NotificationForViewDto> query = from n in _notificationRepo.GetAll().AsNoTracking()
                                                       where n.UserId == userId
                                                       select new NotificationForViewDto
                                                       {
                                                           Id = n.Id,
                                                           Content = n.Content,
                                                       };

            return await query.ToListAsync();
        }
        #endregion

        #region UnReadNotificationNum
        public async Task<int> UnreadNotificationNum(long userId)
        {
            IQueryable<Notification> query = from n in _notificationRepo.GetAll().AsNoTracking()
                                             where n.UserId == userId && n.IsRead == false
                                             select n;

            return await query.CountAsync();
        }
        #endregion
    }
}
