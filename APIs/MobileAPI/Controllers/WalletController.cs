using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
 
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserWallet() 
        {
            var userWallet = await _walletService.GetWalletByUserIdAsync();
            if(userWallet == null)
            {
                return NotFound();
            }
            return Ok(userWallet);
        }
    }
}
