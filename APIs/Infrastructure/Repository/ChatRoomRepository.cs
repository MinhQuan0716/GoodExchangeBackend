using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.ChatRoomModel;
using Application.ViewModel.UserModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ChatRoomRepository : GenericRepository<ChatRoom>, IChatRoomRepository
    {
        private readonly AppDbContext _appDbContext;
        public ChatRoomRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<ChatRoomWithOrder>> GetByUserIdAsync(Guid userId)
        {
            var rooms = await _appDbContext.ChatRooms
                                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                                    .Where(x => x.IsDelete == false)
                                    .Include(c => c.Messages) 
                                    .Include(c => c.Receiver) 
                                    .Include(c => c.Sender)
                                    .ToListAsync();

            // Map entities to DTOs
            var roomDtos = rooms.Select(room => new ChatRoomWithOrder
            {
                roomId = room.Id,
                SenderId = room.SenderId,
                ReceiverId = room.ReceiverId,
                ReceiverName = room.Receiver.UserName,
                SenderName = room.Sender.UserName,
                SenderAvatar = room.Sender.ProfileImage,
                ReceiverAvatar = room.Receiver.ProfileImage,
                // Map other properties as needed
                Messages = room.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                    // Map other properties as needed
                }).ToList(),
                Order = _appDbContext.Orders
                .Where(o => o.Post.CreatedBy == room.ReceiverId)
                .Where(o => o.UserId == room.SenderId).AsSplitQuery().Select(u => new OrderDto
                {
                    OrderId = u.Id,
                    OrderStatusId = u.OrderStatusId
                }).Single()
            }).ToList();
            return roomDtos;
        }

        public async Task<ChatRoomWithOrder> GetMessagesByRoomId(Guid roomId)
        {
            var chatRoom = await _appDbContext.ChatRooms.Where(m => m.Id == roomId).
                                                     Where(x => x.IsDelete == false).
                                                     Include(m => m.Messages)
                                                     .Include(c => c.Sender)
                                                     .Include(c => c.Receiver)
                                                     .FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return null; // Or handle the case when the chat room is not found
            }

            // Fetch users for all CreatedBy user IDs in the messages
            var userIds = chatRoom.Messages
                .Where(m => m.CreatedBy.HasValue)
                .Select(m => m.CreatedBy.Value)
                .Distinct()
                .ToList();

            var users = await _appDbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => new { u.UserName, u.ProfileImage });
            var Order = await _appDbContext.Orders
                .Where(o => o.Post.CreatedBy == chatRoom.ReceiverId)
                .Where(o => o.UserId == chatRoom.SenderId).FirstOrDefaultAsync();
            var roomDto = new ChatRoomWithOrder
            {
                roomId = chatRoom.Id,
                SenderId = chatRoom.SenderId,
                ReceiverId = chatRoom.ReceiverId,
                SenderName = chatRoom.Sender.UserName,
                ReceiverName = chatRoom.Receiver.UserName,
                SenderAvatar = chatRoom.Sender.ProfileImage,  // Add this line
                ReceiverAvatar = chatRoom.Receiver.ProfileImage,  // Add this line
                Messages = chatRoom.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedByUserName = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].UserName
                                        : "Unknown User",
                    Avatar = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].ProfileImage
                                        : "Unknown Avatar",  // Add this line
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                }).OrderBy(m => m.CreatedDate).ToList(),
                Order = new OrderDto
                {
                    OrderId = Order.Id,
                    OrderStatusId = Order.OrderStatusId
                }
            };
            return roomDto;
        }
        
        public async Task<ChatRoomWithOrder> GetRoomBy2UserId(Guid user1, Guid user2)
        {
            var chatRoom = await _appDbContext.ChatRooms.Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
                                                                    (m.SenderId == user2 && m.ReceiverId == user1))
                                                                    .Include(m => m.Messages)
                                                                    .Include(c => c.Sender)
                                                                    .Include(c => c.Receiver).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return null; // Or handle the case when the chat room is not found
            }

            // Fetch users for all CreatedBy user IDs in the messages
            var userIds = chatRoom.Messages
                .Where(m => m.CreatedBy.HasValue)
                .Select(m => m.CreatedBy.Value)
                .Distinct()
                .ToList();

            var users = await _appDbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => new { u.UserName, u.ProfileImage });
            var Order = await _appDbContext.Orders
                .Where(o => o.Post.CreatedBy == chatRoom.ReceiverId)
                .Where(o => o.UserId == chatRoom.SenderId).FirstOrDefaultAsync();
            var roomDto = new ChatRoomWithOrder
            {
                roomId = chatRoom.Id,
                SenderId = chatRoom.SenderId,
                ReceiverId = chatRoom.ReceiverId,
                SenderName = chatRoom.Sender.UserName,
                ReceiverName = chatRoom.Receiver.UserName,
                SenderAvatar = chatRoom.Sender.ProfileImage,  // Add this line
                ReceiverAvatar = chatRoom.Receiver.ProfileImage,  // Add this line
                Messages = chatRoom.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedByUserName = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].UserName
                                        : "Unknown User",
                    Avatar = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].ProfileImage
                                        : "Unknown Avatar",  // Add this line
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                }).OrderBy(m => m.CreatedDate).ToList(),
                Order = new OrderDto
                {
                    OrderId = Order.Id,
                    OrderStatusId = Order.OrderStatusId
                }
            };
            return roomDto;
        }
    }
}
