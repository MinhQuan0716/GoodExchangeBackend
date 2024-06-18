using Application.InterfaceService;
using Application.ViewModel.MessageModel;
using Azure.Messaging;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MobileAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IClaimService _claimService;
        private readonly IMessageService _messageService;
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> UserConnections = new();
        private static readonly ConcurrentDictionary<Guid, ConcurrentBag<Message>> PrivateMessages = new();

        public ChatHub(IClaimService claimService, IMessageService messageService)
        {
            _claimService = claimService;
            _messageService = messageService;
        }

        public async Task SendMessageToUser(Guid recipientUserId, string messageContent)
        {
            var senderUserId = _claimService.GetCurrentUserId;
            if (senderUserId == Guid.Empty || !UserConnections.TryGetValue(recipientUserId.ToString(), out var recipientConnections))
            {
                throw new HubException("Invalid recipient user ID.");
            }

            var createMessageModel = new CreateMessageModel
            {
                SenderId = senderUserId,
                ReceiverId = recipientUserId,
                MessageContent = messageContent
            };

            var success = await _messageService.CreateMessage(createMessageModel);
            if (!success)
            {
                return;
            }

            var msg = new Message
            {
                SenderId = senderUserId,
                ReceiverId = recipientUserId,
                MessageContent = messageContent,
                CreationDate = DateTime.UtcNow
            };

            if (!PrivateMessages.ContainsKey(recipientUserId))
            {
                PrivateMessages[recipientUserId] = new ConcurrentBag<Message>();
            }
            PrivateMessages[recipientUserId].Add(msg);

            foreach (var connectionId in recipientConnections)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderUserId.ToString(), messageContent);
            }
        }

        public async Task<IEnumerable<Message>> GetPrivateMessages(Guid recipientUserId)
        {
            var senderUserId = _claimService.GetCurrentUserId;
            if (senderUserId == Guid.Empty)
            {
                return Enumerable.Empty<Message>();
            }

            if (PrivateMessages.TryGetValue(recipientUserId, out var messages))
            {
                var allMessages = messages.ToList();
                var persistentMessages = await _messageService.GetMessagesBy2UserId(recipientUserId);
                var combinedMessages = allMessages.Concat(persistentMessages)
                    .DistinctBy(m => m.Id)  // Ensure messages are unique by their Id
                    .OrderBy(m => m.CreationDate);  // Ensure messages are sorted by creation date
                return combinedMessages;
            }

            return Enumerable.Empty<Message>();
        }

        public async Task ClosePrivateChat(Guid recipientUserId)
        {
            var senderUserId = _claimService.GetCurrentUserId;
            if (senderUserId == Guid.Empty)
            {
                throw new HubException("Invalid recipient user ID.");
            }

            if (PrivateMessages.TryRemove(recipientUserId, out _))
            {
                // Notify the recipient that the chat is closed
                if (UserConnections.TryGetValue(recipientUserId.ToString(), out var recipientConnections))
                {
                    foreach (var connectionId in recipientConnections)
                    {
                        await Clients.Client(connectionId).SendAsync("ChatClosed", senderUserId.ToString());
                    }
                }
            }
        }

        public override Task OnConnectedAsync()
        {
            var userId = _claimService.GetCurrentUserId.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.AddOrUpdate(userId, _ => new ConcurrentBag<string> { Context.ConnectionId }, (_, bag) => { bag.Add(Context.ConnectionId); return bag; });
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            var userId = _claimService.GetCurrentUserId.ToString();
            if (!string.IsNullOrEmpty(userId) && UserConnections.TryGetValue(userId, out var connections))
            {
                connections.TryTake(out _);
                if (connections.IsEmpty)
                {
                    UserConnections.TryRemove(userId, out _);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}