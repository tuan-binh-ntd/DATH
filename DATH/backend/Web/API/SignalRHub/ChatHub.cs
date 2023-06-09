using AutoMapper;
using Bussiness.Extensions;
using Bussiness.Interface.MessageInterface;
using Bussiness.Interface.MessageInterface.Dto;
using Entities;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageAppService _messageAppService;
        private readonly IMapper _mapper;
        private readonly PresenceTracker _tracker;

        public ChatHub(
            IMessageAppService messageAppService,
            IMapper mapper,
            PresenceTracker tracker
            )
        {
            _messageAppService = messageAppService;
            _mapper = mapper;
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext!.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User!.GetUserName(), otherUser);   
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessageInput input)
        {
            Message message = await _messageAppService.AddMessage(input);
            MessageForViewDto result = _mapper.Map<MessageForViewDto>(message);

            var groupName = GetGroupName(result.SenderUserName!, result.RecipientUserName!);

            //var connections = await _tracker.GetConnectionsForUser(recipient.FirstName + " " + recipient.LastName);
            await Clients.Group(groupName).SendAsync("NewMessage", result);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
