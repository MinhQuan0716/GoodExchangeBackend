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
        [HttpGet]
        public async Task<IActionResult> VnPayRedirect([FromQuery] VnPayResponse vnPayResponse)
        {
            var isUpdated = await _paymentService.HandleIpn(vnPayResponse);
            if (isUpdated.RspCode=="00")
            {
                string exePath = Environment.CurrentDirectory.ToString();
                string FilePath = exePath + @"/PaymentTemplate/PaymentSuccessful.html";
                var htmlContent = await System.IO.File.ReadAllTextAsync(FilePath);
                return Content(htmlContent, "text/html");
            }
            string evnPath = Environment.CurrentDirectory.ToString();
            string filePath = evnPath + @"/PaymentTemplate/PaymentFailed.html";
            var htmlFileContent = await System.IO.File.ReadAllTextAsync(filePath);
            return Content(htmlFileContent, "text/html");
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
