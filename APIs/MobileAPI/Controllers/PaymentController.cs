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
        [HttpGet]
        public async Task<IActionResult> GetPaymentUrl()
        {
            var payemntUrl = _paymentService.GetPayemntUrl();
            if (payemntUrl == null || payemntUrl.Equals(""))
            {
                return BadRequest(payemntUrl);
            }
            return Ok(payemntUrl);
        }
        [HttpGet]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayResponse vnPayResponse)
        {
            var isUpdated = await _paymentService.HandleIpn(vnPayResponse);
            if (isUpdated!=null)
            {
                return Ok(isUpdated);
            }
            return BadRequest(isUpdated);
        }
        [HttpGet]
        public async Task<IActionResult> VnPayRedirect([FromQuery] VnPayResponse vnPayResponse)
        {
            var isUpdated = await _paymentService.HandleIpn(vnPayResponse);
            if (isUpdated != null)
            {
                string redirectUrl = "http://192.168.1.9:8081";
                return Redirect(redirectUrl);
            }
            return BadRequest();
        }
    }
}
