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

        public async Task<List<PostViewModel>> GetAllPostForFilter()
        {
            return await _appDbContext.Posts.Where(x =>x.IsDelete == false)
                                           .Select(x => new PostViewModel
                                           {
                                               PostId = x.Id,
                                               PostContent = x.PostContent,
                                               PostTitle = x.PostTitle,
                                               CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
                                               Product = new ProductModel
                                               {
                                                   ProductName = x.Product.ProductName,
                                                   ProductDescription = x.Product.ProductDescription,
                                                   ProductId = x.ProductId,
                                                   CategoryId = x.Product.CategoryId,
                                                   CategoryName = x.Product.Category.CategoryName,
                                                   ConditionId = x.Product.ConditionId,
                                                   ConditionName = x.Product.ConditionType.ConditionType,
                                                   ProductImageUrl = x.Product.ProductImageUrl,
                                                   ProductPrice = x.Product.ProductPrice,
                                                   ProductStatus = x.Product.ProductStatus,
                                                   RequestedProduct = x.Product.RequestedProduct
                                               }
                                           }).ToListAsync();
        }

        public async Task<List<Post>> GetAllPostsByCreatedByIdAsync(Guid id)
        {
            var posts = await _appDbContext.Posts.Where(p => p.CreatedBy == id)
                .Include(p => p.Product)
                .Include(p => p.Product.Category)
                .Include(p => p.Product.ConditionType)
                .Where(x => x.IsDelete == false)
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
            var postDetail = await _appDbContext.Posts.Where(x => x.Id == postId&&x.IsDelete==false).Select(x => new PostDetailViewModel
            {
                ProductName=x.Product.ProductName,
                ProductDescription=x.Product.ProductDescription,
                ProductImageUrl=x.Product.ProductImageUrl,
                ProductPrice=x.Product.ProductPrice,    
                ProductQuantity=x.Product.ProductQuantity.Value,
                CategoryId=x.Product.CategoryId.Value,
                CategoryName=x.Product.Category.CategoryName,
                ConditionTypeId=x.Product.ConditionId.Value,
                ConditionTypeName=x.Product.ConditionType.ConditionType,
                ProductStatus=x.Product.ProductStatus,
                RequestedProduct=x.Product.RequestedProduct,
                PostAuthor=_appDbContext.Users.Where(user=>user.Id==x.CreatedBy).Select(postAuthor=>new PostAuthor
                {
                    AuthorId=x.CreatedBy.Value,
                    CreatedDate = x.CreationDate.HasValue ? DateOnly.FromDateTime(x.CreationDate.Value) : null,
                    FulName = postAuthor.FirstName+""+postAuthor.LastName,
                    Email=postAuthor.Email,
                    PhoneNumber=postAuthor.PhoneNumber,
                    HomeAddress=postAuthor.HomeAddress,
                    Rating = (float)(postAuthor.RatedUsers.Count() > 0
                    ? (double)postAuthor.RatedUsers.Sum(r => r.RatingPoint) / (postAuthor.RatedUsers.Count())
                    : 0),
                    AuthorImage=postAuthor.VerifyUser.UserImage
                }).Single()
            }).SingleOrDefaultAsync();
            return postDetail;
        }

        public async Task<List<PostViewModel>> SearchPostByProductName(string productName)
        {
            return await _appDbContext.Posts.Where(x=>x.Product.ProductName.Contains(productName)&&x.IsDelete==false)
                                           .Select(x=>new PostViewModel
                                           {
                                               PostId=x.Id,
                                               PostContent=x.PostContent,
                                               PostTitle=x.PostTitle,
                                               CreationDate=DateOnly.FromDateTime(x.CreationDate.Value),
                                               Product=new ProductModel
                                               {
                                                   ProductName=productName,
                                                   ProductDescription=x.Product.ProductDescription,
                                                   ProductId=x.ProductId,
                                                   CategoryId=x.Product.CategoryId,
                                                   CategoryName=x.Product.Category.CategoryName,
                                                   ConditionId=x.Product.ConditionId,
                                                   ConditionName=x.Product.ConditionType.ConditionType,
                                                   ProductImageUrl=x.Product.ProductImageUrl,
                                                   ProductPrice=x.Product.ProductPrice,
                                                   ProductStatus=x.Product.ProductStatus,
                                                   RequestedProduct=x.Product.RequestedProduct
                                               }
                                           }).ToListAsync();
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
