using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    [Route("api/ws/[controller]/[action]")]
    [ApiController]
    public class SocketController : ControllerBase
    {
        private readonly ISocketServerService _socketServerSerice;
        public SocketController(ISocketServerService socketServerSerice)
        {
            _socketServerSerice = socketServerSerice;
        }
        [Authorize]
        [HttpGet]
        public async Task GetPaymentStatus()
        {
            if(HttpContext.WebSockets.IsWebSocketRequest)
            {
                await _socketServerSerice.HandleAsync();
            }
        }
    }
}
