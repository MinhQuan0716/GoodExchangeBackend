using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.RatingModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        private readonly AppDbContext _appDbContext;
        public RatingRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<RatingViewModel>> GetAllRatingByRatedUserId(Guid ratedUserId)
        {
            return await _appDbContext.Ratings.Where(x => x.RatedUserId == ratedUserId && x.IsDelete == false)
                .Select(x=>new RatingViewModel
                {
                    Email=x.Rater.Email,
                    Username=x.Rater.UserName,
                    RatingPoint=x.RatingPoint,
                    RatingReview=x.ReviewContent,
                    RatingTitle=x.RatingTitle
                })
                .ToListAsync();
        }

        public async Task<List<Rating>> GetAllRatingByRaterId(Guid raterId)
        {
            return await _appDbContext.Ratings.Where(x => x.RaterId == raterId&&x.IsDelete==false).ToListAsync();
        }
    }
}
