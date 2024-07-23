using Application.InterfaceService;
using Application.VnPay.Response;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpGet("{choice}")]
        public async Task<IActionResult> GetPaymentUrl(int choice)
        {
            var payemntUrl = _paymentService.GetPayemntUrl(choice);
            if (payemntUrl == null || payemntUrl.Equals(""))
            {
                return BadRequest(payemntUrl);
            }
            return Ok(payemntUrl);
        }
      /*  [HttpGet]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayResponse vnPayResponse)
        {
            var isUpdated = await _paymentService.HandleIpn(vnPayResponse);
            if (isUpdated!=null)
            {
                return Ok(isUpdated);
            }
            return BadRequest(isUpdated);
        }*/
        [HttpGet]
        public async Task<IActionResult> VnPayRedirect([FromQuery] VnPayResponse vnPayResponse)
        {
            var isUpdated = await _paymentService.HandleIpn(vnPayResponse);
            if (isUpdated != null)
            {
                return Ok("Payment success");
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PurchaseSubscription(Guid subscriptionId)
        {
            var isPurchases = await _paymentService.BuySubscription(subscriptionId);
            if (isPurchases)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
