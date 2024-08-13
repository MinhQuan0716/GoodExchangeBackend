using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class PolicyController : BaseController
    {
        private readonly IPolicyService _policyService;
        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPostPrice()
        {
            var post = await _policyService.GetPostPrice();
            return Ok(post);
        }
    }
}
