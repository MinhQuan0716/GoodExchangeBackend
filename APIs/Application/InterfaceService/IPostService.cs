﻿using Application.Common;
using Application.ViewModel.CriteriaModel;
using Application.ViewModel.PostModel;
using Application.ViewModel.WishListModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public  interface IPostService
    {
        Task<bool> BanPost(Guid postId);
        Task<bool> CreatePost(CreatePostModel Post);
        Task<bool> UpdatePost(UpdatePostModel Post);
        Task<bool> DeletePost(Guid PostId);
        Task<Pagination<PostViewModel>> GetAllPost(int pageIndex,int pageSize);
        Task<List<PostViewModel>> GetPostSortByCreationDay();
        Task<List<PostViewModel>> GetPostByCreatedById();
        Task<Pagination<PostViewModel>> SortPostByCategory(int categoryId,int pageIndex,int pageSize);
        Task<bool> AddPostToWishList(Guid postId);
        Task<PostDetailViewModel> GetPostDetailAsync(Guid postId);  
        Task<bool> RemovePostFromFavorite(Guid postId);
        Task<List<WishListViewModel>> SeeAllFavoritePost();
        Task<PostDetailViewModel>GetPostDetailInUserCreatePostList(Guid postId);
        Task<Pagination<PostViewModel>> SearchPostByPostTitle(string postTitle,int pageIndex,int pageSize);
        Task<Pagination<PostViewModel>> FilterPostByProductStatusAndPrice(string producttStatus,string exchangeCondition,int pageIndex,int pageSize);
        Task<bool> CheckIfPostInWishList(Guid postId);
        Task<bool>UnbanPost(Guid postId);
        Task<Pagination<PostViewModel>> GetAllPostForWeb(int pageIndex,int pageSize);
    }
}
