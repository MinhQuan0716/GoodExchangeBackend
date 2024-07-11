using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.ChatRoomModel;
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

        public async Task<List<ChatRoomDto>> GetByUserIdAsync(Guid userId)
        {
            var rooms = await _appDbContext.ChatRooms
                                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                                    .Where(x => x.IsDelete == false)
                                    .Include(c => c.Messages) 
                                    .Include(c => c.Receiver) 
                                    .Include(c => c.Sender)
                                    .ToListAsync();

            // Map entities to DTOs
            var roomDtos = rooms.Select(room => new ChatRoomDto
            {
                roomId = room.Id,
                SenderId = room.SenderId,
                ReceiverId = room.ReceiverId,
                ReceiverName = room.Receiver.UserName,
                SenderName = room.Sender.UserName,
                // Map other properties as needed
                Messages = room.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                    // Map other properties as needed
                }).ToList()
            }).ToList();

            return roomDtos;
        }

        public async Task<ChatRoomDto> GetMessagesByRoomId(Guid roomId)
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
                .ToDictionaryAsync(u => u.Id, u => u.UserName);

            var roomDto = new ChatRoomDto
            {
                roomId = chatRoom.Id,
                SenderId = chatRoom.SenderId,
                ReceiverId = chatRoom.ReceiverId,
                SenderName = chatRoom.Sender.UserName,
                ReceiverName = chatRoom.Receiver.UserName,
                Messages = chatRoom.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedByUserName = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value]
                                        : "Unknown User",
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                    // Map other properties as needed
                }).OrderBy(m => m.CreatedDate).ToList()
            };
            return roomDto;
        }

        public async Task<ChatRoom> GetRoomBy2UserId(Guid user1, Guid user2)
        {
            var room = await _appDbContext.ChatRooms.Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
                                                                    (m.SenderId == user2 && m.ReceiverId == user1)).
                                                                    Where(x => x.IsDelete == false).FirstOrDefaultAsync();
            return room;
        }
    }
}
