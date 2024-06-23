using Application.InterfaceService;
using Application.ViewModel.RatingModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
  
    public class RatingController :BaseController
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RateUser(CreateRatingModel createRatingModel)
        {
            var isRated = await _ratingService.RateUserAsync(createRatingModel);
            if(isRated == false)
            {
                return BadRequest();
            }
            return Ok();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserRatings()
        {
            var lisRatings = await _ratingService.SeeUserRatingList();
            return Ok(lisRatings);
        }
    }
}
