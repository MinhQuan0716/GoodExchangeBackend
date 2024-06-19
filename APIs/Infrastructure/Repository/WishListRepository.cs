using Application.InterfaceRepository;
using Application.InterfaceService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<WishList>> FindWishListByUserId(Guid userId)
        {
            return await _appDbContext.WishLists.Where(x => x.UserId == userId&&x.IsDelete==false).ToListAsync();
        }
    }
}
