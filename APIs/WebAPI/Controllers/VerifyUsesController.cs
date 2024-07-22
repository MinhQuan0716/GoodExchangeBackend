using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllVerifyUsers()
        {
            var listVerifyUser=await _verifyUserService.GetAllWaitingUserToApproveAsync();
            return Ok(listVerifyUser);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ApproveUser([FromRoute]Guid id)
        {
            var isApproved=await _verifyUserService.ApproveImageAsync(id);
            if(isApproved)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> DenyUser([FromRoute] Guid id)
        {
            var isApproved = await _verifyUserService.DenyImageAsync(id);
            if (isApproved)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> detail(Guid id)
        {
            var verfiyDetail=await _verifyUserService.GetVerifyModelDetailByUserIdAsync(id);
            return Ok(verfiyDetail);
        }
    }
}
