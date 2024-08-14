using Application.InterfaceService;
using Application.ViewModel.PolicyModel;
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
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCancelledAmount()
        {
            var policy = await _policyService.GetOrderCancelledTime();
            return Ok(policy);
        }
  
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        public async Task<IActionResult> UpdatePostPrice(PostPriceViewModel postPriceViewModel)
        {
            var isUpdated=await _policyService.UpdatePostPrice(postPriceViewModel);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        //remove id
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        public async Task<IActionResult> UpdateOrderCancelledAmount(OrderCancelledTimeViewModel orderCancelledTimeViewModel)
        {
            var isUpdated = await _policyService.UpdateOrderCancelledTime(orderCancelledTimeViewModel);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
