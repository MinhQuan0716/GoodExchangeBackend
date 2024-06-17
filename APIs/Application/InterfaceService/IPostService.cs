using Application.Common;
using Application.ViewModel.PostModel;
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
        Task<Pagination<PostModel>> GetAllPost(int pageIndex,int pageSize);
        Task<List<PostModel>> GetPostSortByCreationDay();
        Task<List<PostModel>> GetPostByCreatedById();
        Task<List<PostModel>> SortPostByCategory(int categoryId);
        Task<bool> AddPostToWishList(Guid postId);
        Task<PostDetailViewModel> GetPostDetailAsync(Guid postId);  
    }
}
