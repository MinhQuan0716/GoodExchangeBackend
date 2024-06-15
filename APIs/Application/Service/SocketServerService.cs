using Application.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

namespace Application.Service
{
    public class SocketServerService : ISocketServerService
    {
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpListener _httpListener;
        public SocketServerService(IPaymentService paymentService, IHttpContextAccessor httpContextAccessor)
        {
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;

        }
        public void Start()
        {
            string hostUrl = "http://localhost:7777/";
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(hostUrl);
            _httpListener.Start();
            Console.WriteLine("WebSocket server started at ws://localhost:7777");
        }
        public async Task HandleAsync()
        {
            using (var ws = await _httpContextAccessor.HttpContext.WebSockets.AcceptWebSocketAsync())
            {
                try
                {
                    // Start sending payment status updates immediately after WebSocket connection is established
                    var statusSenderTask = SendPaymentStatusUpdatesAsync(ws);

                    // Wait for the WebSocket to close or encounter an error
                    await statusSenderTask;
                }
                catch (WebSocketException wex)
                {
                    Console.WriteLine($"WebSocket Exception: {wex.Message}");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation canceled while handling WebSocket.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
                finally
                {
                    if (ws != null)
                    {
                        if (ws.State == WebSocketState.Open)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                        }
                        ws.Dispose();
                    }
                }
            }
        }
        private async Task SendPaymentStatusUpdatesAsync(WebSocket ws)
        {
            try
            {
                while (ws.State == WebSocketState.Open)
                {
                    int paymentStatus = _paymentService.ReturnTransactionStatus();
                    var statusMessage = Encoding.UTF8.GetBytes(paymentStatus.ToString());

                    await ws.SendAsync(new ArraySegment<byte>(statusMessage), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Wait for a second before checking the status again
                    await Task.Delay(1000);
                    if (paymentStatus > 0)
                    {
                        break;
                    }
                }
            }
            catch (WebSocketException wex)
            {
                Console.WriteLine($"WebSocket Exception: {wex.Message}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation canceled while sending payment status updates.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}

