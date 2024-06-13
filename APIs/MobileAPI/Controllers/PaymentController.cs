using Application.InterfaceService;
using Hangfire;
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
            return Ok(payemntUrl);
        }
        /*[Authorize]
        [HttpGet]
        public IActionResult GetPaymentStatus()
        {
            var paymentStatus = _paymentService.ReturnTransactionStatus();
            if (paymentStatus > 0)
=======
            if (HttpContext.WebSockets.IsWebSocketRequest)
>>>>>>> 58996cc92f48c278166888b8343b14e14eba33ab
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
       
        /*[Authorize]
        [HttpGet]
        public IActionResult GetPaymentStatus()
        {
            *//*int paymentStatus = _paymentService.ReturnTransactionStatus();
            if (paymentStatus > 0)
            {
                return Ok(paymentStatus);
            }
<<<<<<< HEAD
            return BadRequest(paymentStatus);
        }
=======
            return BadRequest();*//*
            
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
    }
}
