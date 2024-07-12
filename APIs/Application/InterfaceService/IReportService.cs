using Application.ViewModel.ReportModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IReportService
    {
        Task<bool> CreateReportAsync(Report report);
        Task<bool> CreateReportForPostAsync(ReportPostModel reportPostModel);
        Task<bool> CreateReportForUserAsync(ReportUserModel reportUserModel);
        Task<List<ReportModel>> GetAllReportsAsync();
    }
}
