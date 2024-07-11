using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.SubcriptionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
  
    public class SubcriptionController : BaseController
    {
        private readonly ISubcriptionService _subcriptionService;
        public SubcriptionController(ISubcriptionService subcriptionService)
        {
            _subcriptionService = subcriptionService;
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult>CreateSubscription(CreateSubcriptionModel createSubcriptionModel)
        {
            bool isCreated = await _subcriptionService.CreateSubcription(createSubcriptionModel);
            if(isCreated) 
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllSubscription()
        {
            var subscriptionList = await _subcriptionService.GetAllSubscriptionAsync();
            return Ok(subscriptionList);
        }
    }
}
