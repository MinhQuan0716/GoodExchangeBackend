using Application.InterfaceService;
using Application.ViewModel.RatingModel;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public RatingService(IUnitOfWork unitOfWork, IClaimService claimService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
            _mapper = mapper;
        }
        public async Task<bool> RateUserAsync(CreateRatingModel createRatingModel)
        {
            var ratingList = await _unitOfWork.RatingRepository.GetAllRatingByRaterId(_claimService.GetCurrentUserId);
            if (ratingList.Where(x => x.RatedUserId == createRatingModel.UserId).Any())
            {
                var updateRating = ratingList.Where(x => x.RatedUserId == createRatingModel.UserId).Single();
                _mapper.Map(createRatingModel, updateRating, typeof(CreateRatingModel), typeof(Rating));
                _unitOfWork.RatingRepository.Update(updateRating);
                return await _unitOfWork.SaveChangeAsync() > 0;
            }
            var rating = _mapper.Map<Rating>(createRatingModel);
            rating.RaterId = _claimService.GetCurrentUserId;
            await _unitOfWork.RatingRepository.AddAsync(rating);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
