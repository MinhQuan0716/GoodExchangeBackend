using Application.InterfaceRepository;
using Application.InterfaceService;
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

        public async Task<List<ChatRoom>> GetByUserIdAsync(Guid userId)
        {
            var room = await _appDbContext.ChatRooms.Where(m => (m.SenderId == userId) || 
                                                                (m.ReceiverId == userId)).
                                                     Where(x => x.IsDelete == false).
                                                     Include(c => c.Messages).ToListAsync();
            return room;
        }

        public async Task<List<Message>> GetMessagesByRoomId(Guid roomId)
        {
            var chatRoom = await _appDbContext.ChatRooms.Where(m => m.Id == roomId).
                                                     Where(x => x.IsDelete == false).
                                                     Include(m => m.Messages).FirstOrDefaultAsync();
            var messages = chatRoom.Messages.OrderBy(m=>m.CreationDate).ToList();
            return messages;
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
