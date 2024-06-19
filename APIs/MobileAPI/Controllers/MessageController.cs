﻿using Application.InterfaceService;
using Application.ViewModel.MessageModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MobileAPI.Hubs;

namespace MobileAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageModel message)
        {
            var isCreated = await _messageService.CreateMessage(message);
            if (isCreated)
            {
                return Ok();
            }
            return BadRequest();
        }

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
    }
}