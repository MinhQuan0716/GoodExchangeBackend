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
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllPost(int pageIndex, int pageSize)
        {
            var post= await _postService.GetAllPostForWeb(pageIndex,pageSize);
            return Ok(post);
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpPatch("{postId}")]
        public async Task<IActionResult> UnbanPost(Guid postId)
        {
            var isUnbanned = await _postService.UnbanPost(postId);
            if (isUnbanned)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet("{postId}")]
        public async Task<IActionResult> PostDetail(Guid postId)
        {
            var postDetail=await _postService.GetPostDetailAsync(postId);
            return Ok(postDetail);
        }
    }
}
