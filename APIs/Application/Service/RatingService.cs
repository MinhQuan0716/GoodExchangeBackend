using Application.InterfaceService;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        public RatingService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }
        public async Task<bool> RateUserAsync(Guid userId, double ratePoint)
        {
            var rating = new Rating
            {
                RatedUserId = userId,
                RaterId=_claimService.GetCurrentUserId,
                RatingPoint=(float)ratePoint
            };
            await _unitOfWork.RatingRepository.AddAsync(rating);
         return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
