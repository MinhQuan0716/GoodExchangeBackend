using Application.InterfaceService;
using Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class SubscriptionHistoryController : BaseController
    {
        private readonly ISubscriptionHistoryService _subscriptionHistoryService;
        public SubscriptionHistoryController(ISubscriptionHistoryService subscriptionHistoryService)
        {
            _subscriptionHistoryService = subscriptionHistoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptionHistory() 
        {
            var subscriptionHistoryList=await _subscriptionHistoryService.GetAllSubscriptionHistoriesAsync();
            return Ok(subscriptionHistoryList);
        }
    }
}
