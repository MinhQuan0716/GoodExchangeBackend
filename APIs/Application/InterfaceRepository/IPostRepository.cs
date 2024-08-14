using Application.Common;
using Application.ViewModel.PostModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IPostRepository:IGenericRepository<Post>
    {
        Task<List<Post>> GetAllPostsWithDetailsAsync();
        Task<List<Post>> GetAllPostsWithDetailsSortByCreationDayAsync(Guid currentUserId);
        Task<List<Post>> GetAllPostsByCreatedByIdAsync(Guid id);
        Task<List<Post>> SortPostByProductCategoryAsync(int categoryId);
        Task<PostDetailViewModel> GetPostDetail(Guid postId);
        Task<List<PostViewModel>> SearchPostByProductName (string productName);
        Task<List<PostViewModel>> GetAllPost(Guid userId);
        Task<Guid> GetProductIdFromPostId(Guid postId);
        Task<List<PostViewModelForWeb>> GetAllPostForWebAsync();
        Task<Post> GetBannedPostById(Guid postId);
        Task<List<PostViewModelForFeaturedImage>>GetFeaturedImagePost();
    }
}
