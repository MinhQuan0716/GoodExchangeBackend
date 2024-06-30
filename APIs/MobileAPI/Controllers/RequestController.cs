using Application.InterfaceService;
using Application.ViewModel.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
  
    public class RequestController : BaseController
    {
        private readonly IRequestService _requestService;
        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendRequestForExchangeOrDonation(CreateRequestModel model)
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
        public async Task<IActionResult> SeeExchangeOrDonationRequest()
        {
            var requestList = await _requestService.GetAllRequestsOfCurrentUserAsync();
            if (requestList.Any())
            {
                return Ok(requestList);
            }
            return BadRequest(ModelState);
        }
    }
}
