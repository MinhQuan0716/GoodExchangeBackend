using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.ReportModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateReportAsync(Report report)
        {
            await _unitOfWork.ReportRepository.AddAsync(report);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<bool> CreateReportForPostAsync(ReportPostModel reportPostModel)
        {
            var report = new Report
            {
                ReportContent = reportPostModel.ReportContent,
                ReportPostId = reportPostModel.ReportPostId
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> CreateReportForUserAsync(ReportUserModel reportUserModel)
        {
            var report = new Report
            {
                ReportContent = reportUserModel.ReportContent,
                ReportUserId = reportUserModel.ReportUserId
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
