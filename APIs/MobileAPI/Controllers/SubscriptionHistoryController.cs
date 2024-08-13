using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
  
    public class SubscriptionHistoryController : BaseController
    {
        private readonly ISubscriptionHistoryService _subscriptionHistoryService;
        public SubscriptionHistoryController(ISubscriptionHistoryService subscriptionHistoryService)
        {
            _subscriptionHistoryService = subscriptionHistoryService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserSubscriptionHistory()
        {
            var subscriptionHistories =await _subscriptionHistoryService.GetAllUsersSubscriptionHistoryDetailAsync();
            return Ok(subscriptionHistories);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserAvailableSubscription() 
        {
            var subscriptionHistories = await _subscriptionHistoryService.GetCurrentUsersAvailableSubscription();
            return Ok(subscriptionHistories);
        }
        [Authorize]
        [HttpDelete("{subscriptionId}")]
        public async Task<IActionResult> UnsubscribeSubscription(Guid subscriptionId)
        {
            bool isUnsubscribe=await _subscriptionHistoryService.UnsubscribeSubscription(subscriptionId);
            if (isUnsubscribe)
            {
                return Ok();
            }
            return BadRequest();    
        }
    }
}
