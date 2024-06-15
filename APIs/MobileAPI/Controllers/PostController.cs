﻿using Application.InterfaceService;
using Application.Service;
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
        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            var posts = await _postService.GetAllPost();
            return Ok(posts);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPostSortByCreationDay()
        {
            var posts = await _postService.GetPostSortByCreationDay();
            return Ok(posts);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPostByCreatedById()
        {
            var posts = await _postService.GetPostByCreatedById();
            return Ok(posts);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SortPostByCategory(int categoryId) 
        {
            var post=await _postService.SortPostByCategory(categoryId);
            return Ok(post);
        }
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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostModel post)
        {
            bool isUpdated = await _postService.UpdatePost(post);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
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

    }
}
