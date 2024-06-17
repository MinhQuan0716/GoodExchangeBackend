using Application.InterfaceService;
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
        public async Task<IActionResult> RateUser(Guid userId,double ratePoint)
        {
            var isRated = await _ratingService.RateUserAsync(userId,ratePoint);
            if(isRated == false)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
