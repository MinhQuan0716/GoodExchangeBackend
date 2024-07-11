using Application.InterfaceService;
using Application.ViewModel.VerifyModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class VerifyUserController : BaseController
    {
        private readonly IVerifyUserService _verifyUserService;
        public VerifyUserController(IVerifyUserService verifyUserService)
        {
            _verifyUserService = verifyUserService;
        }
        [HttpPost]
        public async Task<IActionResult> UploadImageVerifyRequest(IFormFile ImageVerify)
        {
            bool isSuccess = await _verifyUserService.UploadImage(ImageVerify);
            if (isSuccess) { 
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetStatusVerify()
        {
            string Status = await _verifyUserService.getVerifyStatus();
            if (Status == "no verification")
            {
                return BadRequest(Status);
            }
            return Ok(Status);
        }
    }
}
