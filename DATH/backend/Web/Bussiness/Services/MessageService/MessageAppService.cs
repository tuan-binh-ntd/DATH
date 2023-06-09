using AutoMapper;
using Bussiness.Interface.MessageInterface;
using Bussiness.Interface.MessageInterface.Dto;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.MessageService
{
    public class MessageAppService : BaseService, IMessageAppService
    {
        private readonly IRepository<Message, long> _messageRepo;

        public MessageAppService(
            IMapper mapper,
            IRepository<Message, long> messageRepo
            )
        {
            ObjectMapper = mapper;
            _messageRepo = messageRepo;
        }

        public async Task<Message> AddMessage(MessageInput input)
        {
            Message message = ObjectMapper!.Map<Message>(input);

            await _messageRepo.InsertAsync(message);
            return message;
        }

        public async Task DeleteMessage(long id)
        {
            await _messageRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(long userId)
        {
            ICollection<Message> unreadMessages = await (_messageRepo.GetAll().Where(m => m.DateRead == null && m.SenderId == userId).OrderBy(m => m.CreationTime).ToListAsync());
            if(unreadMessages.Any())
            {
                foreach(Message item in unreadMessages)
                {
                    item.DateRead = DateTime.Now;
                }
                await _messageRepo.UpdateRangeAsync(unreadMessages);
            }

            ICollection<Message> messages = await (_messageRepo.GetAll().AsNoTracking().Where(m => m.SenderId == userId).OrderBy(m => m.CreationTime).ToListAsync());
            return messages;
        }
    }
}
