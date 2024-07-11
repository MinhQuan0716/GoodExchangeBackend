using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class VerifyController :BaseController
    {
        private readonly IVerifyUserService _verifyUserService;
        public VerifyController(IVerifyUserService verifyUserService)
        {
            _verifyUserService = verifyUserService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadImageForVerification(IFormFile verifyImage)
        {
            var isUpload=await _verifyUserService.UploadImageForVerifyUser(verifyImage);
            if (isUpload)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
