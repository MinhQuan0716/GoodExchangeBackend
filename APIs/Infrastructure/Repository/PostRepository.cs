using Application.Common;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IDbConnection _dbConnection;
        public PostRepository(AppDbContext appDbContext
            , IClaimService claimService, ICurrentTime currentTime, IDbConnection dbConnection) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
            this._dbConnection = dbConnection;
        }

        public async Task<List<PostViewModel>> GetAllPost(Guid userId)
        {
            return await _appDbContext.Posts.Where(x => x.IsDelete == false&&x.CreatedBy!=userId).Include(x => x.Product)
                                           .ThenInclude(p => p.Category)
                                           .AsSplitQuery()
                                           .Include(x => x.Product)
                                           .ThenInclude(p => p.ConditionType)
                                           .AsSplitQuery()
                                           .Select( x => new PostViewModel
                                           {
                                               PostId = x.Id,
                                               PostContent = x.PostContent,
                                               PostTitle = x.PostTitle,
                                               CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
                                               Location= _appDbContext.Users.Where(u=>u.Id==x.CreatedBy).Select(u=>u.HomeAddress).AsSplitQuery().Single(),
                                               AuthorId=x.CreatedBy.Value,
                                               Product = new ProductModel
                                               {
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
                                           }).AsQueryable().AsNoTracking().ToListAsync();
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
            var postDetail = await _appDbContext.Posts.Where(x => x.Id == postId && x.IsDelete == false)
                                                      .Include(x=>x.Product)
                                                      .ThenInclude(x=>x.Category)
                                                      .AsSplitQuery()
                                                      .Include(x=>x.Product)
                                                      .ThenInclude(x=>x.ConditionType)
                                                      .AsSplitQuery()
                                                      .Select(x => new PostDetailViewModel
            {

                PostId = x.Id,
                PostContent = x.PostContent,
                PostTitle = x.PostTitle,
                ProductImageUrl = x.Product.ProductImageUrl,
                ProductPrice = x.Product.ProductPrice,
                ProductQuantity = x.Product.ProductQuantity.Value,
                CategoryId = x.Product.CategoryId.Value,
                CategoryName = x.Product.Category.CategoryName,
                ConditionTypeId = x.Product.ConditionId.Value,
                ConditionTypeName = x.Product.ConditionType.ConditionType,
                ProductStatus = x.Product.ProductStatus,
                RequestedProduct = x.Product.RequestedProduct,
                PostAuthor = _appDbContext.Users.Where(user => user.Id == x.CreatedBy).Include(user=>user.RatedUsers).AsSplitQuery().Select(postAuthor => new PostAuthor
                {
                    AuthorId = x.CreatedBy.Value,
                    CreatedDate = x.CreationDate.HasValue ? DateOnly.FromDateTime(x.CreationDate.Value) : null,
                    FulName = postAuthor.FirstName + "" + postAuthor.LastName,
                    Email = postAuthor.Email,
                    PhoneNumber = postAuthor.PhoneNumber,
                    HomeAddress = postAuthor.HomeAddress,
                    Rating = (postAuthor.RatedUsers.Count() > 0
                    ? postAuthor.RatedUsers.Sum(r => r.RatingPoint) / (postAuthor.RatedUsers.Count())
                    : 0),
                    AuthorImage = postAuthor.ProfileImage
                }).Single()
            }).SingleOrDefaultAsync();
            return postDetail;
       
        }

        public async Task<List<PostViewModel>> SearchPostByProductName(string productName)
        {
            return await _appDbContext.Posts.Where(x => x.PostTitle.Contains(productName) && x.IsDelete == false).AsSplitQuery()
                                            .Include(x => x.Product).ThenInclude(p => p.Category).AsSplitQuery()
                                            .Include(x => x.Product).ThenInclude(p => p.ConditionType).AsSplitQuery()
                                            .Select(x => new PostViewModel
                                            {
                                                PostId=x.Id,
                                                PostTitle= x.PostTitle,
                                                PostContent= x.PostContent,
                                                CreationDate=DateOnly.FromDateTime(x.CreationDate.Value),
                                                Product=new ProductModel
                                                {
                                                    ProductId=x.ProductId,
                                                    CategoryId=x.Product.CategoryId,
                                                    ConditionId=x.Product.ConditionId,
                                                   CategoryName=x.Product.Category.CategoryName,
                                                   ConditionName=x.Product.ConditionType.ConditionType,
                                                   ProductImageUrl=x.Product.ProductImageUrl,
                                                   ProductPrice=x.Product.ProductPrice,
                                                   ProductStatus=x.Product.ProductStatus,
                                                   RequestedProduct=x.Product.RequestedProduct
                                                }
                                            }).AsQueryable().AsNoTracking().ToListAsync();
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
       

        public async Task<Guid> GetProductIdFromPostId(Guid postId)
        {
            return await _appDbContext.Posts
            .Where(p => p.Id == postId)
            .Select(p => p.Product.Id)
            .FirstOrDefaultAsync();
        }

        public async Task<List<PostViewModel>> GetAllPostForWebAsync()
        {
            return await _appDbContext.Posts.Include(x => x.Product)
                                           .ThenInclude(p => p.Category)
                                           .AsSplitQuery()
                                           .Include(x => x.Product)
                                           .ThenInclude(p => p.ConditionType)
                                           .AsSplitQuery()
                                           .Select(x => new PostViewModel
                                           {
                                               PostId = x.Id,
                                               PostContent = x.PostContent,
                                               PostTitle = x.PostTitle,
                                               CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
                                               Location = _appDbContext.Users.Where(u => u.Id == x.CreatedBy).Select(u => u.HomeAddress).AsSplitQuery().Single(),
                                               AuthorId = x.CreatedBy.Value,
                                               Product = new ProductModel
                                               {
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
                                           }).AsQueryable().AsNoTracking().ToListAsync();
        }

        public async Task<Post> GetBannedPostById(Guid postId)
        {
            return await _appDbContext.Posts.Where(x => x.IsDelete == true && x.Id == postId).SingleAsync();
        }
    }
}

