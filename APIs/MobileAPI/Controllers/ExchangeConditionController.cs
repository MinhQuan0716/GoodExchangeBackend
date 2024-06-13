using Application.InterfaceService;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class ExchangeConditionController : BaseController
    {
        private readonly IExchangeConditionService _exchangeConditionService;
        public ExchangeConditionController(IExchangeConditionService exchangeConditionService)
        {
            _exchangeConditionService = exchangeConditionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllExchangeCondition()
        {
            var ExchangeConditions = await _exchangeConditionService.GetAllExchangeCondition();
            return Ok(ExchangeConditions);
        }
    }
}
