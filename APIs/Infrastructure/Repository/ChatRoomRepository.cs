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
                Id = room.Id,
                SenderId = room.SenderId,
                ReceiverId = room.ReceiverId,
                ReceiverName = room.Receiver.UserName,
                SenderName = room.Sender.UserName,
                // Map other properties as needed
                Messages = room.Messages.Select(message => new MessageDto
                {
                    Id = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = message.CreationDate
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
            var roomDto = new ChatRoomDto
            {
                Id = chatRoom.Id,
                SenderId = chatRoom.SenderId,
                ReceiverId = chatRoom.ReceiverId,
                SenderName = chatRoom.Sender.UserName,
                ReceiverName = chatRoom.Receiver.UserName,
                Messages = chatRoom.Messages.Select(message => new MessageDto
                {
                    Id = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = message.CreationDate
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
