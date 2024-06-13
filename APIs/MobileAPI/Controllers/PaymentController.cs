using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Handles;
using System.Net.WebSockets;

namespace MobileAPI.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly PaymentStatusWebSocketHandler _webSocketHandler;
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _webSocketHandler = new PaymentStatusWebSocketHandler(paymentService);
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
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                await HttpContext.Response.WriteAsync($"{{ \"link\": \"{payemntUrl}\" }}");
                HttpContext.Response.ContentType = "application/json";
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketHandler.HandleAsync(HttpContext, webSocket);
                return new EmptyResult();
            }
            else
            {
                return BadRequest("WebSocket connection required");
            }
        }
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
        /*[Authorize]
        [HttpGet]
        public IActionResult GetPaymentStatus()
        {
            *//*int paymentStatus = _paymentService.ReturnTransactionStatus();
            if (paymentStatus > 0)
            {
                return Ok();
            }
            return BadRequest();*//*
            
        }*/
    }
}
