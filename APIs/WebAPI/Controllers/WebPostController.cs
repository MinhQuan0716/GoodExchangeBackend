using Application.InterfaceService;
using Application.ViewModel.PostModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class WebPostController : BaseController
    {
        private readonly IPostService _postService;
        public WebPostController(IPostService postService)
        {
            _postService = postService;
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> BanPost(Guid postId)
        {
            bool isDelete= await _postService.BanPost(postId);
            if (isDelete)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllPost(int pageIndex, int pageSize)
        {
            var post= await _postService.GetAllPost(pageIndex,pageSize);
            return Ok(post);
        }
    }
}
