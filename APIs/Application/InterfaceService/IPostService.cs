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
        Task<Pagination<PostViewModel>> GetAllPost(int pageIndex,int pageSize);
        Task<List<PostViewModel>> GetPostSortByCreationDay();
        Task<List<PostViewModel>> GetPostByCreatedById();
        Task<List<PostViewModel>> SortPostByCategory(int categoryId);
        Task<bool> AddPostToWishList(Guid postId);
        Task<PostDetailViewModel> GetPostDetailAsync(Guid postId);  
        Task<bool> RemovePostFromFavorite(Guid postId);
        Task<List<WishListViewModel>> SeeAllFavoritePost();
        Task<PostDetailViewModel>GetPostDetailInUserCreatePostList(Guid postId);
        Task<List<PostViewModel>> SearchPostByProductName(string productName);
        Task<List<PostViewModel>> FilterPostByProductStatusAndPrice(PostCriteria postCriteria);
    }
}
