using Application.CacheEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ManagerController : BaseController
    {
        private readonly Setting _setting;
        public ManagerController(Setting setting)
        {
            _setting = setting;
        }
        [Authorize(Roles ="Admin")]
        [HttpPatch]
        public IActionResult UpdatePostPrice(float  price)
        {
           bool isUpdated=_setting.UpdatePostPrice(price);
            if(isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin")]
        [HttpPatch]
        public IActionResult UpdateOrderCancelAmount(int amount)
        {
            bool isUpdated = _setting.UpdateCancelledAmount(amount);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetPostPrice()
        {
            float price= _setting.GetPostPrice();
            if(price>0)
            {
                return Ok(price);
            }
            return BadRequest();
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetOrderCancelAmount()
        {
            int cancelAmount=_setting.GetOrderCancellAmount();
            if (cancelAmount > 0)
            {
                return Ok(cancelAmount);
            }
            return BadRequest();
        }
    }
}
