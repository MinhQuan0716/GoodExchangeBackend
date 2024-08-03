using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
  
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetReceieveOrder()
        {
            var requestList = await _orderService.GetAllRequestsOfCurrentUserAsync();
            if (requestList.Any())
            {
                return Ok(requestList);
            }
            return NotFound();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSendOrder()
        {
            var requestList = await _orderService.GetAllRequestsOfCreatebByUserAsync();
            if (requestList.Any())
            {
                return Ok(requestList);
            }
            return NotFound();
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> AcceptOrder(Guid orderId)
        {
           
                var isAccepted = await _orderService.AcceptRequest(orderId);
                if (isAccepted)
                {
                    return Ok();
                }
               return BadRequest();
            
           
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var orderDetail=await _orderService.GetOrderDetailAsync(orderId);
            if(orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }
        [Authorize]
        [HttpPut()]
        public async Task<IActionResult> UpdateDeliveredOrder(Guid orderId)
        {
           
                var isAccepted = await _orderService.DeliveredOrder(orderId);
                if (isAccepted)
                {
                    return Ok();
                }
                return BadRequest();
            
           
        }
        [Authorize]
        [HttpPut()]
        public async Task<IActionResult> UpdateCancleOrder(Guid orderId)
        {
          
                var isAccepted = await _orderService.CancleOrder(orderId);
                if (isAccepted)
                {
                    return Ok();
                }
                return BadRequest();
            
           
        }
        [Authorize]
        [HttpPut()]
        public async Task<IActionResult> UpdateConfirmOrder(Guid orderId)
        {
            
                var isAccepted = await _orderService.ConfirmOrder(orderId);
                if (isAccepted)
                {
                    return Ok();
                }
                return BadRequest();
          
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllOrderByChatRoomId(Guid roomId)
        {
            
                var orderList = await _orderService.GetAllOrderByChatRoomId(roomId);
                if (orderList.Any())
                {
                    return Ok(orderList);
                }
                return NotFound();
          
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSendOrderByChatRoomId(Guid roomId)
        {
            
                var orderList = await _orderService.GetSendOrderByChatRoomId(roomId);
                if (orderList.Any())
                {
                    return Ok(orderList);
                }
                return NotFound();
            
          
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllOrder()
        {
            
                var orderList = await _orderService.GetAllOrderByCurrentUser();
                if (orderList.Any())
                {
                    return Ok(orderList);
                }
                return NotFound();
        }
    }
}
