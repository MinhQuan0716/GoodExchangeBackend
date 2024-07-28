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
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPatch]
        public async Task<IActionResult> UpdateSubscription(UpdateSubscriptionModel updateSubscriptionModel)
        {
            bool isUpdated=await _subcriptionService.UpdateSubcription(updateSubscriptionModel);
            if(isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllSubscription()
        {
            var subscriptionList = await _subcriptionService.GetAllSubscriptionAsync();
            return Ok(subscriptionList);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{susbcriptionId}")]
        public async Task<IActionResult> DeactiveSubscription(Guid susbcriptionId)
        {
            var isDeactived=await _subcriptionService.DeactiveSubscriptionAsync(susbcriptionId);
            if(isDeactived)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{susbcriptionId}")]
        public async Task<IActionResult> RevokeSubscription(Guid susbcriptionId)
        {
            var isRevoked = await _subcriptionService.RevokeSubscriptionAsync(susbcriptionId);
            if (isRevoked)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
