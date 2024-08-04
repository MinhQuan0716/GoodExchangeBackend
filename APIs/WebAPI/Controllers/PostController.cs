using Application.InterfaceService;
using Application.ViewModel.PostModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class PostController : BaseController
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> BanPost(Guid Id)
        {
            bool isDelete= await _postService.BanPost(Id);
            if (isDelete)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            var post= await _postService.GetAllPostForWeb();
            return Ok(post);
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpPatch("{Id}")]
        public async Task<IActionResult> UnbanPost(Guid Id)
        {
            var isUnbanned = await _postService.UnbanPost(Id);
            if (isUnbanned)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> PostDetail(Guid Id)
        {
            var postDetail=await _postService.GetPostDetailAsync(Id);
            return Ok(postDetail);
        }
    }
}
