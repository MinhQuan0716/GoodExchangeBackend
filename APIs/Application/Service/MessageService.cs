using Application.InterfaceService;
using Application.ViewModel.MessageModel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimService _claimService;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimService = claimService;
        }

        public async Task<bool> CreateMessage(CreateMessageModel messageModel)
        {
            messageModel.SenderId = _claimService.GetCurrentUserId;
            var newMessage = _mapper.Map<Message>(messageModel);
            newMessage.CreationDate = DateTime.UtcNow;
            await _unitOfWork.MessageRepository.AddAsync(newMessage);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> DeleteMessage(Guid messageId)
        {
            var message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
            if (message != null)
            {
                _unitOfWork.MessageRepository.SoftRemove(message);
                return await _unitOfWork.SaveChangeAsync() > 0;
            }
            return false;
        }

        public async Task<List<Message>> GetAllMessages()
        {
            return await _unitOfWork.MessageRepository.GetAllAsync();
        }

        public async Task<Message> GetMessageById(Guid id)
        {
            return await _unitOfWork.MessageRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateMessage(UpdateMessageModel messageModel)
        {
            var updateMessage = _mapper.Map<Message>(messageModel);
            _unitOfWork.MessageRepository.Update(updateMessage);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<List<Message>> GetMessagesBy2UserId(Guid user1)
        {
            Guid user2 = _claimService.GetCurrentUserId;
            return await _unitOfWork.MessageRepository.GetMessagesBy2UserId(user1, user2);
        }
    }
}
