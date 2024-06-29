using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.WishListModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ViewModel.PostModel;
namespace Infrastructure.Repository
{
    public class WishListRepository : GenericRepository<WishList>, IWishListRepository
    {
        private readonly AppDbContext _appDbContext;
        public WishListRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<WishList>> FindWishListByPostId(Guid postId)
        {
            return await _appDbContext.WishLists.Where(x => x.PostId == postId&&x.IsDelete==false).ToListAsync();
        }

        public async Task<List<WishListViewModel>> FindWishListByUserId(Guid userId)
        {
            return await _appDbContext.WishLists.Where(x => x.UserId == userId && x.IsDelete == false)
                .Select(x => new WishListViewModel
                {
                    post = new PostViewModel
                    {
                        PostId = x.Post.Id,
                        CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
                        PostContent = x.Post.PostContent,
                        PostTitle = x.Post.PostTitle,
                        Product = new ProductModel
                        {
                            ProductId=x.Post.Product.Id,
                            CategoryId=x.Post.Product.CategoryId,
                            CategoryName=x.Post.Product.Category.CategoryName,
                            ConditionId=x.Post.Product.ConditionId,
                            ConditionName= x.Post.Product.ConditionType.ConditionType,
                            ProductImageUrl=x.Post.Product.ProductImageUrl,
                            ProductPrice=x.Post.Product.ProductPrice,
                            RequestedProduct=x.Post.Product.RequestedProduct
                        }
                    }
                })
                .ToListAsync();
        }
    }
}
