using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>> GetMessagesBy2UserId(Guid user1, Guid user2);
        Task<Message> getByContent(string messageContent);
    }
}