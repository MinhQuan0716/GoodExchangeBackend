using Application.InterfaceService;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModel.ReportModel;

namespace MobileAPI.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReportForPost([FromForm]ReportPostModel reportPostModel)
        {
            if (reportPostModel == null)
            {
                return BadRequest("ReportPostModel is null");
            }
            bool isCreate = await _reportService.CreateReportForPostAsync(reportPostModel);
            if (isCreate)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReportForUser([FromForm] ReportUserModel reportUserModel)
        {
            if (reportUserModel == null)
            {
                return BadRequest("ReportUserModel is null");
            }

            bool isCreate = await _reportService.CreateReportForUserAsync(reportUserModel);
            if (isCreate)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
