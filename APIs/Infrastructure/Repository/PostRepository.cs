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

        public async Task<List<PostViewModel>> GetAllPostForFilter()
        {
            return await _appDbContext.Posts.Where(x => x.IsDelete == false)
                                           .Select(x => new PostViewModel
                                           {
                                               PostId = x.Id,
                                               PostContent = x.PostContent,
                                               PostTitle = x.PostTitle,
                                               CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
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
            var postDetail = await _appDbContext.Posts.Where(x => x.Id == postId && x.IsDelete == false).Select(x => new PostDetailViewModel
            {
              
                PostId=x.Id,
                PostContent=x.PostContent,
                PostTitle=x.PostTitle,
                ProductImageUrl = x.Product.ProductImageUrl,
                ProductPrice = x.Product.ProductPrice,
                ProductQuantity = x.Product.ProductQuantity.Value,
                CategoryId = x.Product.CategoryId.Value,
                CategoryName = x.Product.Category.CategoryName,
                ConditionTypeId = x.Product.ConditionId.Value,
                ConditionTypeName = x.Product.ConditionType.ConditionType,
                ProductStatus = x.Product.ProductStatus,
                RequestedProduct = x.Product.RequestedProduct,
                PostAuthor = _appDbContext.Users.Where(user => user.Id == x.CreatedBy).Select(postAuthor => new PostAuthor
                {
                    AuthorId = x.CreatedBy.Value,
                    CreatedDate = x.CreationDate.HasValue ? DateOnly.FromDateTime(x.CreationDate.Value) : null,
                    FulName = postAuthor.FirstName + "" + postAuthor.LastName,
                    Email = postAuthor.Email,
                    PhoneNumber = postAuthor.PhoneNumber,
                    HomeAddress = postAuthor.HomeAddress,
                    Rating = (float)(postAuthor.RatedUsers.Count() > 0
                    ? (double)postAuthor.RatedUsers.Sum(r => r.RatingPoint) / (postAuthor.RatedUsers.Count())
                    : 0),
                    AuthorImage = postAuthor.VerifyUser.UserImage
                }).Single()
            }).SingleOrDefaultAsync();
            return postDetail;
            /*  string sql = @" SELECT p.Id,
                      p.PostTitle,
                      p.PostContent,
           p.CreationDate,
           prod.Id AS ProductId,
           prod.ProductName,
           prod.ProductDescription,
           prod.CategoryId,
           cat.CategoryName,
           prod.ConditionId,
           ec.ConditionType ,
           prod.ProductImageUrl,
           prod.ProductPrice,
           prod.ProductStatus,
           prod.RequestedProduct,
           u.Email,
           u.HomeAddress,
           u.PhoneNumber,
    FROM Posts p
    INNER JOIN Products prod ON p.ProductId = prod.Id
    INNER JOIN Categories cat ON prod.CategoryId = cat.CategoryId 
    INNER JOIN ExchangeConditions ec  ON prod.ConditionId=ec.ConditionId
    INNER JOIN Users u ON u.Id=p.CreatedBy
    INNER JOIN Rating r on r.RatedUserId=u.Id
    WHERE p.IsDelete = 0 AND p.Id LIKE @PostId;";
              var parameters = new { PostId = "%" + postId + "%" };
              var queryResult = await _dbConnection.QueryAsync<Post, ProductModel, PostDetailViewModel>
                  (
                  sql,
                  (post, productViewModel) =>
                  {
                      var postDetailViewModel = new PostDetailViewModel() 
                      {
                          ProductDescription = productViewModel.ProductDescription,

                      };


                      return postDetailViewModel;
                  },
                  parameters,
                  splitOn: "ProductId"
                  );
              return queryResult.Single();*/
        }

        public async Task<List<PostViewModel>> SearchPostByProductName(string productName)
        {
            string sql = @"
                     SELECT p.Id,
                    p.PostTitle,
                    p.PostContent,
         p.CreationDate,
         prod.Id AS ProductId,
         prod.CategoryId,
         cat.CategoryName,
         prod.ConditionId,
         ec.ConditionType ,
         prod.ProductImageUrl,
         prod.ProductPrice,
         prod.ProductStatus,
         prod.RequestedProduct
  FROM Posts p
  INNER JOIN Products prod ON p.ProductId = prod.Id
  INNER JOIN Categories cat ON prod.CategoryId = cat.CategoryId 
  INNER JOIN ExchangeConditions ec  ON prod.ConditionId=ec.ConditionId
  WHERE p.IsDelete = 0 AND p.PostTitle LIKE @SearchTerm;";
            var parameters = new { SearchTerm = "%" + productName + "%" };
            var queryResult = await _dbConnection.QueryAsync<Post, ProductModel, PostViewModel>
                (
                sql,
                (post, productViewModel) =>
                {
                    var postViewModel = new PostViewModel();
                    postViewModel.PostId = post.Id;
                    postViewModel.PostContent = post.PostContent;
                    postViewModel.PostTitle = post.PostTitle;
                    postViewModel.CreationDate = DateOnly.FromDateTime(post.CreationDate.Value);
                    postViewModel.Product = new ProductModel
                    {
                        CategoryId = productViewModel.CategoryId,
                        ProductPrice = productViewModel.ProductPrice,
                        CategoryName = productViewModel.CategoryName,
                        ConditionId = productViewModel.ConditionId,
                        ConditionName = productViewModel.ConditionName,
                        ProductId = productViewModel.ProductId,
                        ProductImageUrl = productViewModel.ProductImageUrl,
                        ProductStatus = productViewModel.ProductStatus,
                        RequestedProduct = productViewModel.RequestedProduct,
                    };

                    return postViewModel;
                },
                parameters,
                splitOn: "ProductId"
                );
            return queryResult.ToList();
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
        public async Task<List<PostViewModel>> GetAllPostWithDapper()
        {
            string sql = @"
        SELECT 
            p.Id ,
            p.PostTitle,
            p.PostContent,
            p.CreationDate,
            prod.Id AS ProId,
            prod.CategoryId AS CategoryId,
            cat.CategoryName ,
            prod.ConditionId AS ConditionId,
            ec.ConditionType AS ConditionName,
            prod.ProductImageUrl,
            prod.ProductPrice,
            prod.ProductStatus,
            prod.RequestedProduct
        FROM Posts p
        INNER JOIN Products prod ON p.ProductId = prod.Id
        INNER JOIN Categories cat ON prod.CategoryId = cat.CategoryId 
        INNER JOIN ExchangeConditions ec ON prod.ConditionId = ec.ConditionId
        WHERE p.IsDelete = 0;";

            var postVm = await _dbConnection.QueryAsync<Post, ProductModel,PostViewModel>(
                sql,
                (post, product) =>
                {
                    var postViewModel = new PostViewModel();
                    postViewModel.PostId = post.Id;
                    postViewModel.PostContent = post.PostContent;
                    postViewModel.PostTitle = post.PostTitle;
                    postViewModel.CreationDate = DateOnly.FromDateTime(post.CreationDate.Value);

                    postViewModel.Product = new ProductModel
                    {
                        ProductId = product.ProductId,
                        ProductPrice = product.ProductPrice,
                        CategoryId = product.CategoryId,
                        CategoryName = product.CategoryName,
                        ConditionId = product.ConditionId,
                        ConditionName = product.ConditionName,
                        ProductImageUrl = product.ProductImageUrl,
                        ProductStatus = product.ProductStatus,
                        RequestedProduct = product.RequestedProduct
                    };

                    return postViewModel;
                },
                splitOn: "ProId"  // Split results on these keys
            );

            return postVm.ToList();
        }

        public async Task<Guid> GetProductIdFromPostId(Guid postId)
        {
            return await _appDbContext.Posts
            .Where(p => p.Id == postId)
            .Select(p => p.Product.Id)
            .FirstOrDefaultAsync();
        }
    }
}

