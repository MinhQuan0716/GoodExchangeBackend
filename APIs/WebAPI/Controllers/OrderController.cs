using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
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
        public async Task<IActionResult> GetAllOrder()
        {
            var orderList = await _orderService.GetAllOrderAsync();
            if (orderList.Any())
            {
                return Ok(orderList);
            }
            return NotFound();
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> CancleOrder(Guid orderId)
        {
            var isUpdate = await _orderService.CancleOrderForAdmin(orderId);
            if (isUpdate)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
