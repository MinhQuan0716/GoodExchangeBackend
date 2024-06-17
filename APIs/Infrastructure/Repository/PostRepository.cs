using Application.Common;
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
            /*var post = _appDbContext.Posts.Include(p => p.Product)
                                           .Include(p=>p.Product.Category)
                                           .Include(p=>p.Product.ConditionType)
                                           .AsQueryable();
            var paginationPost=await ToPagination(post,x=>x.IsDelete==false,pageSize,pageIndex);*/
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

        public async Task<PostDetailViewModel> GetPostDetail(Guid postId)
        {
            var postDetail = await _appDbContext.Posts.Where(x => x.Id == postId).Select(x => new PostDetailViewModel
            {
                ProductName=x.Product.ProductName,
                ProductDescription=x.Product.ProductDescription,
                ProductImageUrl=x.Product.ProductImageUrl,
                ProductPrice=x.Product.ProductPrice,
                PostAuthor=_appDbContext.Users.Where(user=>user.Id==x.CreatedBy).Select(postAuthor=>new PostAuthor
                {
                    CreatedDate = x.CreationDate.HasValue ? DateOnly.FromDateTime(x.CreationDate.Value) : null,
                    FulName = postAuthor.FirstName+""+postAuthor.LastName,
                    Email=postAuthor.Email,
                    PhoneNumber=postAuthor.PhoneNumber,
                    HomeAddress=postAuthor.HomeAddress,
                    Rating = (float)(postAuthor.RatedUsers.Count() > 0
                    ? (double)postAuthor.RatedUsers.Sum(r => r.RatingPoint) / (postAuthor.RatedUsers.Count())
                    : 0)
                }).Single()
            }).SingleOrDefaultAsync();
            return postDetail;
        }

        public async Task<List<Post>> SortPostByProductCategoryAsync(int categoryId)
        {
            var listPost = await GetAllAsync(
                p => p.Product,
                p => p.Product.Category,
                p => p.Product.ConditionType);
            var sortedListPost = listPost.Where(p => p.Product.Category.CategoryId == categoryId).ToList();
            return sortedListPost;
        }
    }
}
