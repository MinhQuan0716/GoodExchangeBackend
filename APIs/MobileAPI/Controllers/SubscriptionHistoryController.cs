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
        [HttpDelete("{Id}")]
        public async Task<IActionResult> UnsubscribeSubscription(Guid Id)
        {
            bool isUnsubscribe=await _subscriptionHistoryService.UnsubscribeSubscription(Id);
            if (isUnsubscribe)
            {
                return Ok();
            }
            return BadRequest();    
        }
    }
}
