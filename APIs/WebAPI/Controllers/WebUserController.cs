using Application.InterfaceService;
using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class WebUserController : BaseController
    {
        private readonly IUserService _userService;
        public WebUserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            string apiOrigin = "Web";
            var newToken = await _userService.Login(loginModel,apiOrigin);
            return Ok(newToken);
        }
        [HttpGet]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            bool sendSuccess = await _userService.SendVerificationCodeToEmail(email);
            if (sendSuccess)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CurrentLoginUserInfo()
        {
            var currentUser = await _userService.GetCurrentLoginUserForWeb();
            return Ok(currentUser);
        }
        [HttpGet]
        public IActionResult CheckVerifyCode(string code)
        {
            bool isCorrect = _userService.CheckVerifyCode(code);
            HttpContext.Session.SetString("verifycode", code);
            if (isCorrect)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            string verifycode = HttpContext.Session.GetString("verifycode");
            HttpContext.Session.Clear();
            bool isResetSuccess = await _userService.ResetPassword(verifycode, resetPasswordModel);
            if (isResetSuccess)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string apiOrigin = "Web";
            bool isLogout=await _userService.Logout(apiOrigin);
            if(isLogout)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles="Admin,Moderator")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            bool isBan = await _userService.BanUser(userId);
            if (isBan)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GeAllUser()
        {
            List<User> user=await _userService.GetAllUserAsync();
            return Ok(user);
        }
        [Authorize(Roles ="Admin")]
        [HttpPut]
        public async Task<IActionResult> CreateModerator(Guid userId)
        {
            bool isPromoted=await _userService.PromoteUserToModerator(userId);
            if (isPromoted)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
