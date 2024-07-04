using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IChatRoomRepository : IGenericRepository<ChatRoom>
    {
        Task<List<Message>> GetMessagesByRoomId(Guid roomId);
        Task<ChatRoom> GetRoomBy2UserId(Guid user1, Guid user2);
        Task<List<ChatRoom>> GetByUserIdAsync(Guid userId);
    }
}
