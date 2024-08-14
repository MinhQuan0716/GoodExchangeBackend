using Application.Common;
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
        Task<List<PostViewModel>> GetAllPost();
        Task<List<PostViewModel>> GetPostSortByCreationDay();
        Task<List<PostViewModel>> GetPostByCreatedById();
        Task<List<PostViewModel>> SortPostByCategory(int categoryId,List<PostViewModel>? dataPost);
        Task<bool> AddPostToWishList(Guid postId);
        Task<PostDetailViewModel> GetPostDetailAsync(Guid postId);  
        Task<bool> RemovePostFromFavorite(Guid postId);
        Task<List<WishListViewModel>> SeeAllFavoritePost();
        Task<PostDetailViewModel>GetPostDetailInUserCreatePostList(Guid postId);
        Task<List<PostViewModel>> SearchPostByPostTitleAndFilterPostByProductStatusAndPrice(string postTitile,string productStatus,string exchangeCondition);
        Task<bool> CheckIfPostInWishList(Guid postId);
        Task<bool>UnbanPost(Guid postId);
        Task<List<PostViewModelForWeb>> GetAllPostForWeb();
        Task<List<PostViewModel>> SearchPostCreatedByCurrentUserByPostTitle(string postTitle);
        Task<bool> RemovePostWhenSubscriptionExpire();
        Task<List<PostViewModelForFeaturedImage>> GetFeaturedImage();
    }
}
