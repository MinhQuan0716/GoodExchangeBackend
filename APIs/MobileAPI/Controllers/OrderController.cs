using Application.InterfaceService;
using Application.ViewModel.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
  
    public class OrderController : BaseController
    {
        private readonly IOrderService _requestService;
        public OrderController(IOrderService requestService)
        {
            _requestService = requestService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetReceieveOrder()
        {
            var requestList = await _requestService.GetAllRequestsOfCurrentUserAsync();
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
            var isAccepted = await _requestService.AcceptRequest(orderId);
            if (isAccepted)
            {
                return Ok();
            }
            return BadRequest();  
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var orderDetail=await _requestService.GetOrderDetailAsync(orderId);
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
            var isAccepted = await _requestService.DeliveredOrder(orderId);
            if (isAccepted)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
