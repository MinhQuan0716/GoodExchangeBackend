using Application.InterfaceService;
using Application.ViewModel.ReportModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class VerifyReportController : BaseController
    {
        private readonly IReportService _reportService;

        public VerifyReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]
        public async Task<ActionResult<List<ReportModel>>> GetAllReports()
        {
            var reports = await _reportService.GetAllReportsAsync();
            return Ok(reports);
        }
    }
}
