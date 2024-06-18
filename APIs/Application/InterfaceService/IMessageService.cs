using Application.ViewModel.MessageModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IMessageService
    {
        Task<List<Message>> GetAllMessages();
        Task<Message> GetMessageById(Guid id);
        Task<bool> CreateMessage(CreateMessageModel message);
        Task<bool> UpdateMessage(UpdateMessageModel message);
        Task<bool> DeleteMessage(Guid id);
        Task<List<Message>> GetMessagesBy2UserId(Guid user1);
    }
}
