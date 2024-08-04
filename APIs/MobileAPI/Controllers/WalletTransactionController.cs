using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class WalletTransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        public WalletTransactionController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTransactionList()
        {
            var listTransaction = await _walletTransactionService.GetAllTransactionByCurrentUserIdAsync();
            return Ok(listTransaction);
        }
    }
}
