using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.PostModel;
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
                ReportPostId = reportPostModel.postId
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> CreateReportForUserAsync(ReportUserModel reportUserModel)
        {
            var report = new Report
            {
                ReportContent = reportUserModel.ReportContent,
                ReportUserId = reportUserModel.authorId
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<List<ReportModel>> GetAllReportsAsync()
        {
            var reports = await _unitOfWork.ReportRepository.GetAllAsync(report => report.ReportUser, report => report.ReportPost, 
                report => report.ReportPost.Product, report => report.ReportPost.Product.Category, report => report.ReportPost.Product.ConditionType);

            var reportModels = reports.Select(report => new ReportModel
            {
                Id = report.Id,
                ReportContent = report.ReportContent,
                ReportUserId = report.ReportUserId,
                ReportPostId = report.ReportPostId,
                user = report.ReportUser == null ? null : new UserViewModelForReport
                {
                    userId = report.ReportUser.Id,
                    Username = report.ReportUser.UserName,
                    Email = report.ReportUser.Email,
                    ImageUrl = report.ReportUser.ProfileImage,
                    HomeAddress = report.ReportUser.HomeAddress
                },
                post = report.ReportPost == null ? null : new PostDetailViewModelForReport
                {
                    PostId = report.ReportPost.Id,
                    PostTitle = report.ReportPost.PostTitle,
                    PostContent = report.ReportPost.PostContent,
                    ProductImageUrl = report.ReportPost.Product.ProductImageUrl,
                    ProductPrice = report.ReportPost.Product.ProductPrice,
                    ProductStatus = report.ReportPost.Product.ProductStatus,
                    ProductQuantity = report.ReportPost.Product.ProductQuantity,
                    ConditionTypeId = report.ReportPost.Product.ConditionType.ConditionId,
                    ConditionTypeName = report.ReportPost.Product.ConditionType.ConditionType,
                    CategoryId = report.ReportPost.Product.Category.CategoryId,
                    CategoryName = report.ReportPost.Product.Category.CategoryName
                }
            }).ToList();

            return reportModels;
        }
    }
}
