using Application.InterfaceService;
using Application.Util;
using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Services;

namespace MobileAPI.Controllers
{

    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ISendMailHelper _sendMailHelper;
        public UserController(IUserService userService, ISendMailHelper sendMailHelper)
        {
            _userService = userService;
            _sendMailHelper = sendMailHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
                var isCreate = await _userService.CreateAccount(registerModel);
                if (!isCreate)
                {
                    return BadRequest();
                }
                return Ok();
           
        }
        /// <summary>
        /// Api For Login
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            string apiOrigin = "Mobile";
            var newToken = await _userService.Login(loginModel, apiOrigin);
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
            bool isResetSuccess = await _userService.ResetPassword(verifycode, resetPasswordModel);
            if (isResetSuccess)
            {
                HttpContext.Session.Clear();
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string apiOrigin = "Mobile";
            bool isLogout = await _userService.Logout(apiOrigin);
            if (isLogout)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> LoginGoogle(string Token)
        {
            string apiOrigin = "Mobile";
            var newToken = await _userService.LoginGoogle(Token, apiOrigin);
            if (newToken == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(newToken);
            }
        }
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
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            List<User> users = await _userService.GetAllUserAsync();
            return Ok(users);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm]UpdateUserProfileModel updateUserProfileModel)
        {
            bool isUpdate = await _userService.UpdateUserProfileAsync(updateUserProfileModel);
            if (isUpdate)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentLoginUser()
        {
            var user = await _userService.GetCurrentLoginUser();
            return Ok(user);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordModel updatePasswordModel)
        {
            bool updatePassword=await _userService.UpdatePasswordAsync(updatePasswordModel);
            if (!updatePassword)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
