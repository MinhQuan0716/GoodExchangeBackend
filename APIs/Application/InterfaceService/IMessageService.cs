﻿using Application.ViewModel.MessageModel;
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
        Task<Message> CreateMessage(CreateMessageModel message);
        Task<bool> UpdateMessage(UpdateMessageModel message);
        Task<bool> DeleteMessage(Guid id);
        Task<ChatRoom> GetOrCreateChatRoomAsync(Guid receiverId);
        Task<List<Message>> GetMessagesByChatRoomId(Guid chatRoomId);
        Task<ChatRoom> GetChatRoomByIdAsync(Guid chatRoomId);
        Task<List<ChatRoom>> GetAllChatRoomsByUserIdAsync(Guid chatRoomId);
    }
}
