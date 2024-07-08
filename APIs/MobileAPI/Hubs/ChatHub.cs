using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.MessageModel;
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
            if (senderUserId == Guid.Empty)
            {
                throw new HubException("Invalid sender user ID.");
            }

            var chatRoom = await _messageService.GetOrCreateChatRoomAsync(recipientUserId);
            if (chatRoom == null)
            {
                throw new HubException("Unable to create or retrieve chat room.");
            }

            var createMessageModel = new CreateMessageModel
            {
                MessageContent = messageContent,
                RoomId = chatRoom.Id
            };

            var message = await _messageService.CreateMessage(createMessageModel);
            if (message.CreatedBy == null)
            {
                return;
            }

            var messages = PrivateMessages.GetOrAdd(chatRoom.Id, _ => new ConcurrentBag<Message>());
            messages.Add(message);

            if (UserConnections.TryGetValue(recipientUserId.ToString(), out var recipientConnections))
            {
                foreach (var connectionId in recipientConnections)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderUserId.ToString(), messageContent);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("RecipientNotConnected", recipientUserId.ToString());
            }
        }

        public async Task<IEnumerable<Message>> GetPrivateMessages(Guid chatRoomId)
        {
            var userId = _claimService.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                return Enumerable.Empty<Message>();
            }

            if (PrivateMessages.TryGetValue(chatRoomId, out var messages))
            {
                var allMessages = messages.ToList();
                var persistentMessages = await _messageService.GetMessagesByChatRoomId(chatRoomId);
                var combinedMessages = allMessages.Concat(persistentMessages)
                    .DistinctBy(m => m.Id)
                    .OrderBy(m => m.CreationDate);
                return combinedMessages;
            }

            return Enumerable.Empty<Message>();
        }

        public async Task ClosePrivateChat(Guid chatRoomId)
        {
            var userId = _claimService.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                throw new HubException("Invalid user ID.");
            }

            if (PrivateMessages.TryRemove(chatRoomId, out _))
            {
                var chatRoom = await _messageService.GetChatRoomByIdAsync(chatRoomId);
                if (chatRoom != null)
                {
                    var recipientUserId = chatRoom.SenderId == userId ? chatRoom.ReceiverId : chatRoom.SenderId;
                    if (UserConnections.TryGetValue(recipientUserId.ToString(), out var recipientConnections))
                    {
                        foreach (var connectionId in recipientConnections)
                        {
                            await Clients.Client(connectionId).SendAsync("ChatClosed", userId.ToString());
                        }
                    }
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _claimService.GetCurrentUserId.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.AddOrUpdate(userId, _ => new ConcurrentBag<string> { Context.ConnectionId }, (_, bag) => { bag.Add(Context.ConnectionId); return bag; });

                // Await the task to get the list of chat rooms
                var userChatRooms = await _messageService.GetAllChatRoomsByUserIdAsync(Guid.Parse(userId));
                foreach (var chatRoom in userChatRooms)
                {
                    if (PrivateMessages.TryGetValue(chatRoom.Id, out var messages))
                    {
                        foreach (var message in messages)
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", message.CreatedBy.ToString(), message.MessageContent);
                        }
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
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
        public async Task<IEnumerable<ChatRoom>> GetAllRooms()
        {
            var userId = _claimService.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                throw new HubException("Invalid user ID.");
            }

            var chatRooms = await _messageService.GetAllChatRoomsByUserIdAsync(userId);
            return chatRooms;
        }
    }
}