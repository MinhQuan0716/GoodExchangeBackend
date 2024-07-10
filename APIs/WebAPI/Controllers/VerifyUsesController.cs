using Application.InterfaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    public class VerifyUsesController : BaseController
    {
        private readonly IVerifyUserService _verifyUserService;
        public VerifyUsesController(IVerifyUserService verifyUserService)
        {
            _verifyUserService = verifyUserService;
        }
        [HttpGet]
        public async Task<IActionResult> VerifyUsers()
        {
            var listVerifyUser=await _verifyUserService.GetAllWaitingUserToApproveAsync();
            return Ok(listVerifyUser);
        }
    }
}
