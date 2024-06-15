using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.PostModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly AppDbContext _appDbContext;
        public PostRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Post>> GetAllPostsByCreatedByIdAsync(Guid id)
        {
            var posts = await _appDbContext.Posts.Where(p => p.CreatedBy == id)
                .Include(p => p.Product)
                .Include(p => p.Product.Category)
                .Include(p => p.Product.ConditionType)
                .ToListAsync();
            return posts;
        }

        public async Task<List<Post>> GetAllPostsWithDetailsAsync()
        {
            var posts = await GetAllAsync(
                p => p.Product,
                p => p.Product.Category,
                p => p.Product.ConditionType
            );

            return posts;
        }

        public async Task<List<Post>> GetAllPostsWithDetailsSortByCreationDayAsync()
        {
            var posts = await GetAllAsync(
                p => p.Product,
                p => p.Product.Category,
                p => p.Product.ConditionType
            );
            var sortedPosts = posts.OrderBy(p => p.CreationDate).ToList();

            return sortedPosts;

        }

        public async Task<List<Post>> SortPostByProductCategoryAsync(int categoryId)
        {
            var listPost = await GetAllAsync(
                p => p.Product,
                p => p.Product.Category,
                p => p.Product.ConditionType);
            var sortedListPost= listPost.Where(p=>p.Product.Category.CategoryId == categoryId).ToList();
            return sortedListPost;
        }
    }
}
