using Application.InterfaceService;
using System.Net.WebSockets;
using System.Text;

namespace MobileAPI.Handles
{
    public class PaymentStatusWebSocketHandler
    {
        private readonly IPaymentService _paymentService;

        public PaymentStatusWebSocketHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task HandleAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                int paymentStatus = _paymentService.ReturnTransactionStatus();
                var statusMessage = Encoding.UTF8.GetBytes(paymentStatus.ToString());

                await webSocket.SendAsync(new ArraySegment<byte>(statusMessage, 0, statusMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                // Wait for a second before checking the status again
                await Task.Delay(1000);
                if (paymentStatus > 0)
                {
                    break;
                }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
