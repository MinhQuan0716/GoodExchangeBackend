using Application.InterfaceService;
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
        public IActionResult GetPaymentUrl()
        {
            var payemntUrl = _paymentService.GetPayemntUrl();
            if (payemntUrl == null || payemntUrl.Equals(""))
            {
                return BadRequest(payemntUrl);
                
            }
            /*RecurringJob.AddOrUpdate<>(() => ())*/;
            return Ok(payemntUrl);
        }
        /*[Authorize]
        [HttpGet]
        public IActionResult GetPaymentStatus()
        {
            var paymentStatus = _paymentService.ReturnTransactionStatus();
            if (paymentStatus > 0)
            {
                return Ok(paymentStatus);
            }
            return BadRequest(paymentStatus);
        }*/
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUserBalance()
        {
            bool isAdded = await _paymentService.AddMoneyToWallet();
            if (isAdded)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserRefund()
        {
            bool refundResult = await _paymentService.Refund();
            if (refundResult)
            {
                return Ok(refundResult);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetPaymentStatus()
        {
            int paymentStatus = _paymentService.ReturnTransactionStatus();
            if (paymentStatus > 0)
            {
                return Ok(paymentStatus);
            }
            return BadRequest(paymentStatus);
        }
    }
}
