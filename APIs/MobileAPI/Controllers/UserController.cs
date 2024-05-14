﻿using Application.InterfaceService;
using Application.ViewModel.UserViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Services;

namespace MobileAPI.Controllers
{

    public class UserController :BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult>Register (RegisterModel registerModel)
        {
            var isCreate=await _userService.CreateAccount(registerModel);
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
            var newToken=await _userService.Login(loginModel, apiOrigin);
            return Ok(newToken);
        }
        [HttpGet]
        public async Task<IActionResult> SendVerificationCode (string email)
        {
            bool sendSuccess= await _userService.SendVerificationCodeToEmail(email);
            if(sendSuccess)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        public IActionResult CheckVerifyCode(string code)
        {
            bool isCorrect=  _userService.CheckVerifyCode(code);
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
            bool isResetSuccess= await _userService.ResetPassword(verifycode,resetPasswordModel);
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
            string apiOrigin = "Mobile";
            bool isLogout = await _userService.Logout(apiOrigin);
            if(isLogout)
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
            if(newToken == null)
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
            List<User> users= await _userService.GetAllUserAsync();
            return Ok(users);
        }
    }
}
