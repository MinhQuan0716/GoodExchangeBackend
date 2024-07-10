using Application.InterfaceService;
using Application.ViewModel.MessageModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MobileAPI.Hubs;

namespace MobileAPI.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageService.GetAllMessages();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(Guid id)
        {
            var message = await _messageService.GetMessageById(id);
            if (message == null)
            {
                return NotFound();
            }
            return Ok(message);
        }
        /*[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageModel message)
        {
            var isCreated = await _messageService.CreateMessage(message);
            if (isCreated)
            {
                return Ok();
            }
            return BadRequest();
        }*/

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageModel message)
        {
            var isUpdated = await _messageService.UpdateMessage(message);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var isDeleted = await _messageService.DeleteMessage(id);
            if (isDeleted)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> TestSendMessageToUser(Guid recipientUserId, Guid Id, string messageContent)
        {
            var senderUserId = Id;

            var chatRoom = await _messageService.GetOrCreateChatRoomAsync(recipientUserId);
            if (chatRoom == null)
            {
                throw new HubException("Unable to create or retrieve chat room.");
            }

            var createMessageModel = new CreateMessageModel
            {
                MessageContent = messageContent,
                RoomId = chatRoom.Id
            };

            var message = await _messageService.CreateMessage(createMessageModel);
            if (message.CreatedBy == null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var userChatRooms = await _messageService.GetAllChatRoomsByUserIdAsync();
            return Ok(userChatRooms);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrivateMessagesByChatRoomId(Guid id)
        {
            var allMessages = await _messageService.GetMessagesByChatRoomId(id);
            return Ok(allMessages);
        }
    }
}