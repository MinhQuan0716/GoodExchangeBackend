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
        [HttpPost]
        public async Task<IActionResult> SendOrder(CreateOrderModel model)
        {
            bool isCreated=await _requestService.SendRequest(model);
            if (isCreated)
            {
                return Ok();
            }
            return BadRequest(ModelState);  
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
        [HttpGet]   
        public async Task<IActionResult> GetSentOrder()
        {
            var requestList = await _requestService.GetAllRequestsOfCreatebByUserAsync();
            if (requestList.Any())
            {
                return Ok(requestList);
            }
            return NotFound();
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> AcceptOrder(Guid requestId)
        {
            var isAccepted = await _requestService.AcceptRequest(requestId);
            if (isAccepted)
            {
                return Ok();
            }
            return BadRequest();  
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> RejectOrder(Guid requestId)
        {
            var isRejected = await _requestService.RejectRequest(requestId);
            if (isRejected)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
