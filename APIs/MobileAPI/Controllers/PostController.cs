using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.CriteriaModel;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class PostController : BaseController
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        /// <summary>
        /// Api get all post for main page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            var posts = await _postService.GetAllPost();
       /*     if(posts.Items.Count() == 0)
            {
                return NotFound();
            }*/
            return Ok(posts);
        }
        /// <summary>
        /// Sort post by  createtion day
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPostSortByCreationDay()
        {
            var posts = await _postService.GetPostSortByCreationDay();
            return Ok(posts);
        }
        /// <summary>
        /// Get all post for My Posts page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPostByCurrentUserId()
        {
            var posts = await _postService.GetPostByCreatedById();
            return Ok(posts);
        }
        /// <summary>
        /// Sort post by product category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        /*[Authorize]*/
        [HttpGet]
        public async Task<IActionResult> SortPostByCategory(int categoryId) 
        {
            var post=await _postService.SortPostByCategory(categoryId);
            return Ok(post);
        }
        /// <summary>
        /// Create post with product info
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostModel post)
        {
            bool isCreate = await _postService.CreatePost(post);
            if (isCreate)
            {
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Update post or modify product info
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostModel post)
        {
            bool isUpdated = await _postService.UpdatePost(post);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Remove post by post id
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemovePost(Guid postId)
        {
            bool isRemoved = await _postService.DeletePost(postId);
            if (isRemoved)
            {
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Api add post to wishlist
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavoritePost(Guid postId)
        {
            bool isAdded=await _postService.AddPostToWishList(postId);
            if (isAdded)
            {
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Api get post detail for main page
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPostDetail(Guid postId)
        {
            var postDetail=await _postService.GetPostDetailAsync(postId);
            if (postDetail == null)
            {
                return NotFound();
            }
            return Ok(postDetail);
        }
        /// <summary>
        /// Api remove post in wishlist
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemovePostFromFavoriteList(Guid postId)
        {
            bool isRemoved=await _postService.RemovePostFromFavorite(postId);
            if (isRemoved)
            {
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Api see all post in wishlist
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPostInFavoriteList()
        {
            var favoritePostList = await _postService.SeeAllFavoritePost();
            return Ok(favoritePostList);    
        }
        /// <summary>
        /// Api get post detail for My Post page
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult>GetPostDetailInListPostCreatedByUser(Guid postId)
        {
            var postDetail = await _postService.GetPostDetailAsync(postId);
            if (postDetail == null)
            {
                return NotFound();
            }
            return Ok(postDetail);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchAndFilterPost(string? postTitle,string? productStatus,string? exchangeCondition)
        {
            var filterListPost=await _postService.SearchPostByPostTitleAndFilterPostByProductStatusAndPrice(postTitle,productStatus,exchangeCondition);
            if(filterListPost.Count() == 0)
            {
                return NotFound();
            }
            return Ok(filterListPost);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult>SearchPostCurrentUser(string postTitle)
        {
            var searchPostList=await _postService.SearchPostCreatedByCurrentUserByPostTitle(postTitle);
            return Ok(searchPostList);
        }
        [Authorize]
        [HttpGet]   
        public async Task<IActionResult>CheckPostIfExistInWishList(Guid postId)
        {
            var isExisted = await _postService.CheckIfPostInWishList(postId);
            if(!isExisted)
            {
                return Ok(isExisted);
            }
            return Ok(isExisted);
        }
    }
}
