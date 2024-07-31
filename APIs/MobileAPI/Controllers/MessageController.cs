using Application.InterfaceService;
using Application.Service;
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
        private readonly IOrderService _orderService;
        private readonly IPostService _postService;
        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext, IOrderService orderService, IPostService postService)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _orderService = orderService;
            _postService = postService;
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
            var checkOrderStatus = await _orderService.CheckOrderStatusByPostId(postId);
            if (checkOrderStatus)
            {
                return BadRequest("Post already get accept by other user");
            }
            var checkPost = await _postService.GetPostDetailAsync(postId);
            if (checkPost.PostAuthor.AuthorId == userId)
            {
                return BadRequest("User can not order their own post");
            }
            var userChatRooms = await _messageService.GetOrCreateChatRoomAsync(userId, postId);
            if (userChatRooms == null)
            {
                return BadRequest("userId not exist");
            }
            return Ok(userChatRooms);
        }
    }
}