using Bussiness.Interface.MessageInterface.Dto;
using Entities;

namespace Bussiness.Interface.MessageInterface
{
    public interface IMessageAppService
    {
        public Task<Message> AddMessage(MessageInput input);
        public Task DeleteMessage(long id);
        public Task<IEnumerable<Message>> GetMessageThread(long userId);
    }
}
