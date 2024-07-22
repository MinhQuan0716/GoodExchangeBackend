using Application.ViewModel.ChatRoomModel;
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
        Task<ChatRoomWithOrder> GetMessagesByRoomId(Guid roomId);
        Task<ChatRoomWithOrder> GetRoomBy2UserId(Guid user1, Guid user2);
        Task<List<ChatRoomDto>> GetByUserIdAsync(Guid userId);
    }
}
