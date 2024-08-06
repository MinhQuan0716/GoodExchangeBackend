using Application.InterfaceService;
using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
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
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            List<UserViewModelForWeb> user=await _userService.GetAllUserForWeb();
            return Ok(user);
        }
        [Authorize(Roles ="Admin")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> CreateModerator(Guid userId)
        {
            bool isPromoted=await _userService.PromoteUserToModerator(userId);
            if (isPromoted)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UnbanUser(Guid userId)
        {
            bool isUnban=await _userService.UnBanUserAsync(userId);
            if (isUnban)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet("{id}")]
        public async Task<IActionResult> detail(Guid id)
        {
            var useDetail = await _userService.GetUserInformation(id);
            return Ok(useDetail);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentUser = await _userService.GetCurrentLoginUserForWeb();
            return Ok(currentUser);
        }
    }
}
