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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var userChatRooms = await _messageService.GetAllChatRoomsByUserIdAsync();
            return Ok(userChatRooms);
        }

        [Authorize]
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetPrivateMessagesByChatRoomId(Guid roomId)
        {
            var allMessages = await _messageService.GetMessagesByChatRoomId(roomId);
            return Ok(allMessages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ContactNow(Guid userId, Guid postId)
        {
            var userChatRooms = await _messageService.GetOrCreateChatRoomAsync(userId, postId);
            return Ok(userChatRooms);
        }
    }
}