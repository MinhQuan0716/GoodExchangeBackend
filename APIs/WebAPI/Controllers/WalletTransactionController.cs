using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class WalletTransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        public WalletTransactionController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet]
        public async Task<IActionResult> TransactionList()
        {
            var listTransaction =await _walletTransactionService.GetAllTransactionAsync();
            return Ok(listTransaction);
        }
    }
}
