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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> CancleOrder(Guid Id)
        {
            var isUpdate = await _orderService.CancleOrderForAdmin(Id);
            if (isUpdate)
            {
                return Ok();
            }
            return NotFound();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid Id)
        {
            var orderDetail = await _orderService.GetOrderDetailAsync(Id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }
    }
}
