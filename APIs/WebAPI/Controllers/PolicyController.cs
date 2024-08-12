using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class PolicyController :BaseController
    {
        private readonly IPolicyService _policyService;
        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPostPrice()
        {
            var post=await _policyService.GetPostPrice();
            return Ok(post);
        }
        [HttpGet]
        public async Task<IActionResult> GetCancelledAmount()
        {
            var policy = await _policyService.GetOrderCancelledTime();
            return Ok(policy);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePostPrice(Guid id,float price)
        {
            var isUpdated=await _policyService.UpdatePostPrice(id,price);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrderCancelledAmount(Guid id,int amount)
        {
            var isUpdated = await _policyService.UpdateOrderCancelledTime(id, amount);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
